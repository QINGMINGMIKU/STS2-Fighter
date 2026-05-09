using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Keywords;

namespace Fighter;

[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "Fighter";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);

    public static void Init()
    {
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);

        FighterKeywords.Register(ModKeywordRegistry.For(ModId));

        RitsuLibFramework.CreateContentPack(ModId)
            .Character<FighterCharacter>()
            .Apply();

        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        RitsuLibFramework.SubscribeLifecycle<SideTurnStartingEvent>(OnSideTurnStarting);

        Logger.Info("Fighter mod initialized!");
    }

    private static void OnSideTurnStarting(SideTurnStartingEvent e)
    {
        TurnState.Reset();

        if (e.Side != CombatSide.Player) return;

        foreach (var creature in e.CombatState.Allies)
        {
            var song = creature.GetPower<DevilsSongPower>();
            if (song == null || song.Amount <= 0) continue;

            var remaining = song.Amount - 1;
            if (remaining <= 0)
                song.RemoveInternal();
            else
                song.SetAmount(remaining);
        }
    }
}
