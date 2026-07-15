using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Combat.SecondaryResources;

namespace Fighter;

public static class CounterHitState
{
    public static bool PlayerPunishCounterReady { get; set; }
    public static int PlayerDamageTakenThisTurn { get; set; }
    public static int PlayerDamageBlockedThisTurn { get; set; }

    public static bool PlayerHasNegativeFrames(Player player)
    {
        return FrameHelper.Get(player) < 0;
    }

    public static void OnTurnEnd()
    {
        PlayerPunishCounterReady = false;
    }
}
