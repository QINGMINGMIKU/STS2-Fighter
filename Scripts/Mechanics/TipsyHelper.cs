using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Fighter;

public static class TipsyHelper
{
    public const int MaxTipsy = 4;

    public static int GetEffectiveTipsy(Creature creature)
    {
        var song = creature.GetPower<DevilsSongPower>();
        if (song != null && song.Amount > 0)
            return MaxTipsy;
        return creature.GetPower<Tipsy>()?.Amount ?? 0;
    }
}
