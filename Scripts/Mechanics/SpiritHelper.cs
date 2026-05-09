using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Fighter;

public static class SpiritHelper
{
    public const float InsufficientPenalty = 0.75f;

    /// <summary>
    /// Consume <paramref name="cost"/> Fighting Spirit from the player.
    /// Returns true if spirit was sufficient, false if insufficient
    /// (all remaining consumed, -25% penalty applies).
    /// </summary>
    public static async Task<bool> SpendSpirit(PlayerChoiceContext ctx, Player player, int cost)
    {
        var spirit = player.Creature.GetPower<FightingSpirit>();
        if (spirit == null || spirit.Amount <= 0)
            return false;

        if (spirit.Amount >= cost)
        {
            await PowerCmd.ModifyAmount(ctx, spirit, -cost, player.Creature, null, false);
            return true;
        }

        await PowerCmd.ModifyAmount(ctx, spirit, -spirit.Amount, player.Creature, null, false);
        return false;
    }
}
