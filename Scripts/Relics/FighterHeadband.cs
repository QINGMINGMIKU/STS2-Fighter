using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
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

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == null || dealer == null || Owner == null)
            return 1m;

        var playerCreature = Owner.Creature;
        if (playerCreature == null)
            return 1m;

        var (multiplier, frames, frameTarget) = CalculateCounterHit(dealer, target, playerCreature);
        if (multiplier > 1f)
        {
            _pendingFrames = frames;
            _pendingFrameTarget = frameTarget;
            return (decimal)multiplier;
        }
        return 1m;
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

        // Combo: set Strike_H cost to 0
        var comboActive = Owner?.Creature.GetPower<Combo>() is { Amount: > 0 };
        if (comboActive && Owner?.PlayerCombatState?.Hand != null)
        {
            foreach (var card in Owner.PlayerCombatState.Hand.Cards)
            {
                if (card is Strike_H)
                    card.EnergyCost.SetThisTurnOrUntilPlayed(0);
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
        Creature source, Creature target, Creature playerCreature)
    {
        if (source == playerCreature && CounterHitState.PlayerPunishCounterReady)
        {
            CounterHitState.PlayerPunishCounterReady = false;
            return (DamageMultiplier, PunishCounterFrames, source);
        }

        if (source == playerCreature && target != playerCreature
            && target.IsMonster && target.Monster?.NextMove?.Intents?.OfType<AttackIntent>().Any() == true)
        {
            var bonusFrames = playerCreature.GetPower<WhiffPunish>() != null ? 2 : 0;
            return (DamageMultiplier, CounterHitFrames + bonusFrames, source);
        }

        if (target == playerCreature
            && source != playerCreature
            && (playerCreature.GetPower<FrameAdvantage>()?.Amount ?? 0) < 0)
        {
            return (DamageMultiplier, CounterHitFrames, source);
        }

        return (1f, 0, null);
    }

    private async Task GrantFrameAdvantage(PlayerChoiceContext choiceContext, Creature target, int frames)
    {
        if (frames == 0)
            return;

        if (target != Owner?.Creature && target is not IFighterTagged)
            return;

        await PowerCmd.Apply<FrameAdvantage>(choiceContext, target, frames, target, null);
    }
}
