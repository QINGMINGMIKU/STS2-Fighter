using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Fighter;

public static class CounterHitState
{
    public static bool PlayerPunishCounterReady { get; set; }
    public static int PlayerDamageTakenThisTurn { get; set; }
    public static int PlayerDamageBlockedThisTurn { get; set; }

    public static bool PlayerHasNegativeFrames(Player player)
    {
        var frames = player.Creature.GetPower<FrameAdvantage>()?.Amount ?? 0;
        return frames < 0;
    }

    public static void OnTurnEnd()
    {
        PlayerPunishCounterReady = false;
    }
}
