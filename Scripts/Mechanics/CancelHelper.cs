using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Fighter;

public static class CancelHelper
{
    public static async Task<bool> ConsumeCancel(PlayerChoiceContext ctx, Creature creature)
    {
        var cancel = creature.GetPower<Cancel>();
        if (cancel == null || cancel.Amount <= 0)
            return false;

        await PowerCmd.Remove(cancel);
        return true;
    }

    public static async Task ClearCancel(PlayerChoiceContext ctx, Creature creature)
    {
        var cancel = creature.GetPower<Cancel>();
        if (cancel != null && cancel.Amount > 0)
            await PowerCmd.Remove(cancel);
    }
}
