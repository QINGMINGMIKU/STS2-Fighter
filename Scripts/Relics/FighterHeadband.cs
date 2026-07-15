using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterRelic(typeof(FighterRelicPool))]
[RegisterCharacterStarterRelic(typeof(FighterCharacter))]
public sealed class FighterHeadband : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const float DamageMultiplier = 1.20f;
    private const int CounterHitFrames = 2;
    private const int PunishCounterFrames = 4;

    private Creature? _pendingFrameTarget;
    private int _pendingFrames;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/relics/fighter_headband.png",
        IconOutlinePath: "Fighter/images/relics/fighter_headband_outline.png",
        BigIconPath: "Fighter/images/relics/fighter_headband_big.png"
    );

    private const float CriticalArtHpThreshold = 0.25f;
    private const float CriticalArtDamageBonus = 1.10f;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay = null)
    {
        if (target == null || dealer == null || Owner == null)
            return 1m;

        var playerCreature = Owner.Creature;
        if (playerCreature == null)
            return 1m;

        var result = 1m;

        // Counter-hit multiplier
        var (counterHitMult, frames, frameTarget) = CalculateCounterHit(dealer, target, Owner);
        if (counterHitMult > 1f)
        {
            _pendingFrames = frames;
            _pendingFrameTarget = frameTarget;
            result = (decimal)counterHitMult;
        }

        // Critical Art: Super cards at ≤25% HP deal +10% damage
        if (cardSource != null && dealer == playerCreature && HasSuperKeyword(cardSource))
        {
            if (playerCreature.CurrentHp > 0 && (float)playerCreature.CurrentHp / playerCreature.MaxHp <= CriticalArtHpThreshold)
                result *= (decimal)CriticalArtDamageBonus;
        }

        return result;
    }

    private static bool HasSuperKeyword(CardModel card)
    {
        if (FighterKeywords.Super == null) return false;
        return card.Keywords.Contains(FighterKeywords.Super.CardKeywordValue);
    }

    private static bool HasCancelKeyword(CardModel card)
    {
        if (FighterKeywords.Cancel == null) return false;
        return card.Keywords.Contains(FighterKeywords.Cancel.CardKeywordValue);
    }

    private static bool HasComboKeyword(CardModel card)
    {
        if (FighterKeywords.Combo == null) return false;
        return card.Keywords.Contains(FighterKeywords.Combo.CardKeywordValue);
    }

    private static bool HasStarterKeyword(CardModel card)
    {
        if (FighterKeywords.Starter == null) return false;
        return card.Keywords.Contains(FighterKeywords.Starter.CardKeywordValue);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        await GrantPendingFrames(choiceContext);

        if (dealer == null || Owner == null || target != Owner.Creature)
            return;

        if (dealer != Owner.Creature && result.WasFullyBlocked)
            CounterHitState.PlayerPunishCounterReady = true;

        if (target == Owner.Creature && result.TotalDamage > 0)
        {
            CounterHitState.PlayerDamageTakenThisTurn += result.TotalDamage;
            if (result.WasFullyBlocked)
                CounterHitState.PlayerDamageBlockedThisTurn += result.TotalDamage;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await GrantPendingFrames(choiceContext);

        if (Owner?.PlayerCombatState?.Hand != null)
        {
            var comboActive = Owner.Creature.GetPower<Combo>() is { Amount: > 0 };

            if (comboActive)
            {
                // Set all Strike_H to 0 cost while Combo is active
                foreach (var card in Owner.PlayerCombatState.Hand.Cards)
                {
                    if (card is Strike_H)
                        card.EnergyCost.SetThisTurnOrUntilPlayed(0);
                }
            }
            else
            {
                // Restore canonical cost when Combo is gone
                foreach (var card in Owner.PlayerCombatState.Hand.Cards)
                {
                    if (card is Strike_H)
                        card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                }
            }
        }

        // Cancel: resets to zero after playing a non-Cancel card
        if (cardPlay.Card != null && !HasCancelKeyword(cardPlay.Card))
        {
            Godot.GD.Print($"[Fighter] ClearCancel: card={cardPlay.Card.GetType().Name}");
            await CancelHelper.ClearCancel(choiceContext, Owner!.Creature);
        }

        // Combo: clear after non-Starter, non-Combo card
        if (cardPlay.Card != null && !HasComboKeyword(cardPlay.Card) && !HasStarterKeyword(cardPlay.Card))
        {
            var combo = Owner!.Creature.GetPower<Combo>();
            if (combo is { Amount: > 0 })
            {
                Godot.GD.Print($"[Fighter] ClearCombo: card={cardPlay.Card.GetType().Name}");
                await PowerCmd.Remove(combo);
            }
        }

        // Super Art Talisman: count attacks
        if (cardPlay.Card != null && cardPlay.Card.Type == CardType.Attack)
        {
            foreach (var relic in Owner!.Relics)
            {
                if (relic is SuperArtTalisman talisman)
                {
                    talisman.IncrementAttackCounter();
                    break;
                }
            }
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;
        CounterHitState.PlayerDamageTakenThisTurn = 0;
        CounterHitState.PlayerDamageBlockedThisTurn = 0;
        await CancelHelper.ClearCancel(choiceContext, player.Creature);
    }

    private async Task GrantPendingFrames(PlayerChoiceContext choiceContext)
    {
        if (_pendingFrames > 0 && _pendingFrameTarget != null)
        {
            await GrantFrameAdvantage(choiceContext, _pendingFrameTarget, _pendingFrames);
            _pendingFrames = 0;
            _pendingFrameTarget = null;
        }
    }

    private static (float multiplier, int frames, Creature? frameTarget) CalculateCounterHit(
        Creature source, Creature target, Player player)
    {
        var playerCreature = player.Creature;

        if (source == playerCreature && CounterHitState.PlayerPunishCounterReady)
        {
            CounterHitState.PlayerPunishCounterReady = false;
            return (DamageMultiplier, PunishCounterFrames, source);
        }

        if (source == playerCreature && target != playerCreature
            && target.IsMonster
            && target.Monster?.NextMove != null
            && target.Monster.NextMove.Intents?.OfType<AttackIntent>().Any() == true)
        {
            var bonusFrames = playerCreature.GetPower<WhiffPunish>()?.Amount ?? 0;
            return (DamageMultiplier, CounterHitFrames + bonusFrames, source);
        }

        if (target == playerCreature
            && source != playerCreature
            && FrameHelper.Get(player) < 0)
        {
            return (DamageMultiplier, CounterHitFrames, source);
        }

        return (1f, 0, null);
    }

    private async Task GrantFrameAdvantage(PlayerChoiceContext choiceContext, Creature target, int frames)
    {
        if (frames <= 0 || Owner == null)
            return;

        // Only grant frames to the player
        if (target == Owner.Creature)
            await FrameHelper.Gain(Owner, frames);
    }
}
