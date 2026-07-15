using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Combat.SecondaryResources;

namespace Fighter;

public static class SpecialHelper
{
    public const int SpiritCost = 2;

    public static bool CanPlaySpecial(CardModel card, Player player)
    {
        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        if (spirit >= SpiritCost)
            return true;

        var energyCost = card.EnergyCost.Canonical;
        var currentEnergy = player.PlayerCombatState?.Energy ?? 0;
        if (currentEnergy < energyCost)
            return false;

        var frames = FrameHelper.Get(player);
        return frames >= energyCost * 2;
    }

    public static async Task<int> PaySpecialCost(Player player, CardModel card)
    {
        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        if (spirit >= SpiritCost)
        {
            await SpiritHelper.SpendSpirit(player, SpiritCost);
            return 0;
        }

        var energyCost = card.EnergyCost.Canonical;
        var frames = FrameHelper.Get(player);
        if (frames >= energyCost * 2)
            await FrameHelper.Lose(player, energyCost * 2);

        return energyCost;
    }
}
