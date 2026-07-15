using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Combat.SecondaryResources;

namespace Fighter;

/// <summary>
/// Convenience wrapper around SecondaryResourceCmd for the FrameAdvantage resource.
/// Auto-refreshes combat UI gauges on change.
/// </summary>
public static class FrameHelper
{
    public static int Get(Player player)
        => SecondaryResourceCmd.Get(player, FighterResources.FrameAdvantage);

    public static async Task Gain(Player player, int amount)
    {
        await SecondaryResourceCmd.Gain(player, FighterResources.FrameAdvantage, amount);
        FighterCombatUiActivatePatch.Refresh(player);
    }

    public static async Task Lose(Player player, int amount)
    {
        await SecondaryResourceCmd.Lose(player, FighterResources.FrameAdvantage, amount);
        FighterCombatUiActivatePatch.Refresh(player);
    }
}
