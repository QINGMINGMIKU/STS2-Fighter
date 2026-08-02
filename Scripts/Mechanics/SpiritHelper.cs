using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Combat.SecondaryResources;

namespace Fighter;

public static class SpiritHelper
{
    public const float InsufficientPenalty = 0.75f;

    /// <summary>
    /// Consume <paramref name="cost"/> Fighting Spirit from the player.
    /// Returns true if spirit was sufficient, false if insufficient.
    /// </summary>
    public static async Task<bool> SpendSpirit(Player player, int cost)
    {
        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        if (spirit <= 0)
            return false;

        if (spirit >= cost)
        {
            await SecondaryResourceCmd.Lose(player, FighterResources.FightingSpirit, cost);
            return true;
        }

        await SecondaryResourceCmd.Lose(player, FighterResources.FightingSpirit, spirit);
        return false;
    }
}
