using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

/// <summary>
/// Innate Fighter passive — Fighting Spirit, Burnout, Combo clear at turn end.
/// Counter-hit and card-play hooks live in FighterHeadband relic.
/// </summary>
[RegisterPower]
public sealed class FighterInnatePower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    // ── Fighting Spirit ──
    private const int InitialSpirit = 6;
    private const int SpiritPerTurn = 1;
    private const int BurnoutTurns = 2;
    private const int BurnoutWeak = 2;
    private const int BurnoutVuln = 2;
    private const int BurnoutRefill = 6;
    private int _burnoutRemaining;
    private bool _spiritInitialized;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/fighting_spirit.png",
        BigIconPath: "Fighter/images/powers/fighting_spirit_big.png"
    );

    // ═══════════════════════
    //  Turn start (spirit + clear combo/cancel)
    // ═══════════════════════

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return;

        // Clear Combo at end of turn
        var combo = Owner!.GetPower<Combo>();
        if (combo is { Amount: > 0 })
            await PowerCmd.Remove(combo);

        await CancelHelper.ClearCancel(choiceContext, Owner);

        // ── Fighting Spirit ──
        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);

        if (!_spiritInitialized)
        {
            _spiritInitialized = true;
            if (spirit <= 0)
                await SecondaryResourceCmd.Set(player, FighterResources.FightingSpirit, InitialSpirit);
            _burnoutRemaining = 0;
            await ApplyStatBonus(choiceContext, player);
            FighterCombatUiActivatePatch.Refresh(player);
            return;
        }

        if (_burnoutRemaining > 0)
        {
            _burnoutRemaining--;
            if (_burnoutRemaining == 0)
            {
                await SecondaryResourceCmd.Set(player, FighterResources.FightingSpirit, BurnoutRefill);
                await ApplyStatBonus(choiceContext, player);
            }
            FighterCombatUiActivatePatch.Refresh(player);
            return;
        }

        if (spirit <= 0)
        {
            await TriggerBurnout(choiceContext, player);
            FighterCombatUiActivatePatch.Refresh(player);
            return;
        }

        if (spirit < 6)
            await SecondaryResourceCmd.Gain(player, FighterResources.FightingSpirit,
                Math.Min(SpiritPerTurn, 6 - spirit));

        await ApplyStatBonus(choiceContext, player);
        FighterCombatUiActivatePatch.Refresh(player);
    }

    // ═══════════════════════
    //  Fighting Spirit helpers
    // ═══════════════════════

    private static async Task ApplyStatBonus(PlayerChoiceContext choiceContext, Player player)
    {
        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        if (spirit <= 0) return;

        var existingStr = player.Creature.GetPower<StrengthPower>();
        var existingDex = player.Creature.GetPower<DexterityPower>();
        if (existingStr == null)
            await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, 1, player.Creature, null);
        else if (existingStr.Amount < 1)
            await PowerCmd.ModifyAmount(choiceContext, existingStr, 1 - existingStr.Amount, player.Creature, null, false);
        if (existingDex == null)
            await PowerCmd.Apply<DexterityPower>(choiceContext, player.Creature, 1, player.Creature, null);
        else if (existingDex.Amount < 1)
            await PowerCmd.ModifyAmount(choiceContext, existingDex, 1 - existingDex.Amount, player.Creature, null, false);
    }

    private async Task TriggerBurnout(PlayerChoiceContext choiceContext, Player player)
    {
        _burnoutRemaining = BurnoutTurns;
        var str = player.Creature.GetPower<StrengthPower>();
        var dex = player.Creature.GetPower<DexterityPower>();
        if (str is { Amount: > 0 })
            await PowerCmd.ModifyAmount(choiceContext, str, -1, player.Creature, null, false);
        if (dex is { Amount: > 0 })
            await PowerCmd.ModifyAmount(choiceContext, dex, -1, player.Creature, null, false);
        await PowerCmd.Apply<WeakPower>(choiceContext, player.Creature, BurnoutWeak, player.Creature, null);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, player.Creature, BurnoutVuln, player.Creature, null);
    }
}
