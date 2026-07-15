using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Keywords;
using STS2RitsuLib.Patching.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

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

        RegisterSecondaryResources();

        // RitsuLib-native patches for combat UI gauges
        var patcher = RitsuLibFramework.CreatePatcher(ModId, "ui", "Fighter UI");
        patcher.RegisterPatch<FighterCombatUiActivatePatch>();
        patcher.RegisterPatch<FighterCombatUiAnimOutPatch>();
        patcher.RegisterPatch<FighterCombatUiDeactivatePatch>();
        patcher.PatchAll();

        RitsuLibFramework.CreateContentPack(ModId)
            .Character<FighterCharacter>()
            .Apply();

        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        RitsuLibFramework.SubscribeLifecycle<SideTurnStartingEvent>(OnSideTurnStarting);

        Logger.Info("Fighter mod initialized!");
    }

    private static void RegisterSecondaryResources()
    {
        var registry = ModSecondaryResourceRegistry.For(ModId);

        registry.Register(FighterResources.FrameAdvantage, new SecondaryResourceDefinition(
            defaultAmount: 0,
            minAmount: int.MinValue,
            hardMaxAmount: int.MaxValue,
            turnStartPolicy: SecondaryResourceTurnStartPolicy.None,
            persistencePolicy: SecondaryResourcePersistencePolicy.Combat
        ));
        registry.AlwaysShowInCombatUiForCharacter<FighterCharacter>(FighterResources.FrameAdvantage);

        registry.Register(FighterResources.SuperGauge, new SecondaryResourceDefinition(
            defaultAmount: 0,
            baseMaxAmount: 3,
            hardMaxAmount: 3,
            minAmount: 0,
            turnStartPolicy: SecondaryResourceTurnStartPolicy.None,
            persistencePolicy: SecondaryResourcePersistencePolicy.Run
        ));
        registry.AlwaysShowInCombatUiForCharacter<FighterCharacter>(FighterResources.SuperGauge);

        registry.Register(FighterResources.FightingSpirit, new SecondaryResourceDefinition(
            defaultAmount: 0,
            baseMaxAmount: 6,
            hardMaxAmount: 6,
            minAmount: 0,
            turnStartPolicy: SecondaryResourceTurnStartPolicy.None,
            persistencePolicy: SecondaryResourcePersistencePolicy.Combat
        ));
        registry.AlwaysShowInCombatUiForCharacter<FighterCharacter>(FighterResources.FightingSpirit);
    }

    private static void OnSideTurnStarting(SideTurnStartingEvent e)
    {
        TurnState.Reset();

        if (e.Side != CombatSide.Player) return;

        foreach (var creature in e.CombatState.Allies)
        {
            // Apply innate Fighter passive only to Fighter character
            if (creature is { IsPlayer: true, Player.Character: FighterCharacter }
                && creature.GetPower<FighterInnatePower>() == null)
            {
                _ = PowerCmd.Apply<FighterInnatePower>(
                    new ThrowingPlayerChoiceContext(), creature, 1, creature, null);
            }

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
