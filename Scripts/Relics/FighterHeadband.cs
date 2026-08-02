using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
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

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/relics/fighter_headband.png",
        IconOutlinePath: "Fighter/images/relics/fighter_headband_outline.png",
        BigIconPath: "Fighter/images/relics/fighter_headband_big.png"
    );

    private const float CriticalArtHpThreshold = 0.25f;
    private const float CriticalArtDamageBonus = 1.10f;

    // ═══════════════════════
    //  Damage modifier — consume counter-hit marks on attack
    // ═══════════════════════

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay = null)
    {
        if (target == null || dealer == null || Owner == null)
            return 1m;

        var playerCreature = Owner.Creature;
        if (playerCreature == null)
            return 1m;

        var result = 1m;

        // Player attacking an enemy → consume counter-hit mark (确反康优先)
        // Only consume on real damage, not during card targeting preview (cardPlay == null)
        if (cardPlay != null && dealer == playerCreature && target != playerCreature)
        {
            // 确反康 — Punish Counter (fully blocked): higher priority, +4 frames
            var punish = target.GetPower<PunishCounterPower>();
            if (punish != null)
            {
                _ = PowerCmd.Remove(punish);
                _ = FrameHelper.Gain(Owner, PunishCounterPower.BonusFrames);
                result = (decimal)PunishCounterPower.DamageMultiplier;
            }
            // 打康 — Counter Hit (enemy intends to Attack): +2 frames + WhiffPunish
            else if (target.GetPower<CounterHitPower>() is { } counter)
            {
                _ = PowerCmd.Remove(counter);
                var bonusFrames = playerCreature.GetPower<WhiffPunish>()?.Amount ?? 0;
                _ = FrameHelper.Gain(Owner, CounterHitPower.BonusFrames + bonusFrames);
                result = (decimal)CounterHitPower.DamageMultiplier;
            }
        }

        // 打康 A2 — enemy counter-hits player at negative frames
        if (target == playerCreature && dealer != playerCreature && dealer.IsMonster
            && FrameHelper.Get(Owner) < 0)
        {
            result = (decimal)CounterHitPower.DamageMultiplier;
        }

        // Critical Art: Super cards at ≤25% HP deal +10% damage
        if (cardSource != null && dealer == playerCreature && HasSuperKeyword(cardSource))
        {
            if (playerCreature.CurrentHp > 0 && (float)playerCreature.CurrentHp / playerCreature.MaxHp <= CriticalArtHpThreshold)
                result *= (decimal)CriticalArtDamageBonus;
        }

        return result;
    }

    // ═══════════════════════
    //  Defensive hook — apply Punish Counter mark on full block
    // ═══════════════════════

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer == null || Owner == null || target != Owner.Creature || dealer == Owner.Creature)
            return;

        // Fully blocked → apply/refresh Punish Counter to attacker (max 1)
        if (result.WasFullyBlocked)
        {
            var existing = dealer.GetPower<PunishCounterPower>();
            if (existing != null)
                _ = PowerCmd.Remove(existing);
            _ = PowerCmd.Apply<PunishCounterPower>(
                new ThrowingPlayerChoiceContext(), dealer, 1, Owner.Creature, null);
        }
    }

    // ═══════════════════════
    //  Card hooks — Combo, Cancel, Super Art
    // ═══════════════════════

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Starter keyword: auto-apply 1 Combo
        if (Owner != null && cardPlay.Card != null && HasStarterKeyword(cardPlay.Card))
            _ = PowerCmd.Apply<Combo>(choiceContext, Owner.Creature, 1, Owner.Creature, cardPlay.Card);

        if (Owner?.PlayerCombatState?.Hand != null)
        {
            var comboActive = Owner.Creature.GetPower<Combo>() is { Amount: > 0 };

            if (comboActive)
            {
                foreach (var card in Owner.PlayerCombatState.Hand.Cards)
                {
                    if (card is Strike_H)
                        card.EnergyCost.SetThisTurnOrUntilPlayed(0);
                }
            }
            else
            {
                foreach (var card in Owner.PlayerCombatState.Hand.Cards)
                {
                    if (card is Strike_H)
                        card.EnergyCost.SetThisTurnOrUntilPlayed(card.EnergyCost.Canonical);
                }
            }
        }

        // Cancel: clears after playing a non-Cancel card
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
        // Cancel clearing is handled by FighterInnatePower
    }

    // ═══════════════════════
    //  Keyword helpers
    // ═══════════════════════

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
}
