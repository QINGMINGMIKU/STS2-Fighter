using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterRelic(typeof(FighterRelicPool))]
[RegisterCharacterStarterRelic(typeof(FighterCharacter))]
public sealed class SpiritCharm : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const int InitialSpirit = 6;
    private const int SpiritPerTurn = 1;
    private const int BurnoutTurns = 2;
    private const int BurnoutWeak = 2;
    private const int BurnoutVuln = 2;
    private const int BurnoutRefill = 6;

    private int _burnoutRemaining;
    private bool _combatInitialized;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/relics/spirit_charm.png",
        IconOutlinePath: "Fighter/images/relics/spirit_charm_outline.png",
        BigIconPath: "Fighter/images/relics/spirit_charm_big.png"
    );

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner) return;

        if (!_combatInitialized)
        {
            _combatInitialized = true;
            _burnoutRemaining = 0;
            await PowerCmd.Apply<FightingSpirit>(choiceContext, player.Creature, InitialSpirit, player.Creature, null);
            await ApplyStatBonus(choiceContext, player);
            Flash();
            return;
        }

        if (_burnoutRemaining > 0)
        {
            _burnoutRemaining--;
            if (_burnoutRemaining == 0)
            {
                await PowerCmd.Apply<FightingSpirit>(choiceContext, player.Creature, BurnoutRefill, player.Creature, null);
                await ApplyStatBonus(choiceContext, player);
                Entry.Logger.Info("[Spirit] Burnout ended — refilled to 6.");
            }
            return;
        }

        var spirit = player.Creature.GetPower<FightingSpirit>();
        if (spirit == null || spirit.Amount <= 0)
        {
            await TriggerBurnout(choiceContext, player);
            return;
        }

        await PowerCmd.Apply<FightingSpirit>(choiceContext, player.Creature, SpiritPerTurn, player.Creature, null);
        await ApplyStatBonus(choiceContext, player);
        Flash();
    }

    private static async Task ApplyStatBonus(PlayerChoiceContext choiceContext, Player player)
    {
        var spirit = player.Creature.GetPower<FightingSpirit>();
        if (spirit != null && spirit.Amount > 0)
        {
            var existingStr = player.Creature.GetPower<StrengthPower>();
            var existingDex = player.Creature.GetPower<DexterityPower>();
            if (existingStr == null || existingStr.Amount < 1)
                await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, 1, player.Creature, null);
            if (existingDex == null || existingDex.Amount < 1)
                await PowerCmd.Apply<DexterityPower>(choiceContext, player.Creature, 1, player.Creature, null);
        }
    }

    private static async Task RemoveStatBonus(PlayerChoiceContext choiceContext, Player player)
    {
        var str = player.Creature.GetPower<StrengthPower>();
        var dex = player.Creature.GetPower<DexterityPower>();
        if (str != null && str.Amount > 0)
            await PowerCmd.ModifyAmount(choiceContext, str, -1, player.Creature, null, false);
        if (dex != null && dex.Amount > 0)
            await PowerCmd.ModifyAmount(choiceContext, dex, -1, player.Creature, null, false);
    }

    private async Task TriggerBurnout(PlayerChoiceContext choiceContext, Player player)
    {
        _burnoutRemaining = BurnoutTurns;
        await RemoveStatBonus(choiceContext, player);

        await PowerCmd.Apply<WeakPower>(choiceContext, player.Creature, BurnoutWeak, player.Creature, null);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, player.Creature, BurnoutVuln, player.Creature, null);

        Entry.Logger.Info($"[Spirit] Burnout triggered — {BurnoutTurns} turns of Weak+Vuln.");
    }
}
