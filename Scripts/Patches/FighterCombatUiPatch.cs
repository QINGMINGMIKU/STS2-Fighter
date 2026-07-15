using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Patching.Models;

namespace Fighter;

/// <summary>
/// RitsuLib-native patch on NCombatUi.Activate to create our custom gauge nodes.
/// </summary>
public sealed class FighterCombatUiActivatePatch : IPatchMethod
{
    public static string PatchId => "fighter_combat_ui_gauge";
    public static string Description => "Add FighterSuperGauge and FighterFrameCounter to combat UI";
    public static bool IsCritical => false;

    /// <summary>Weak ref to current combat UI instance for manual refresh.</summary>
    public static NCombatUi? CurrentUi { get; private set; }

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(NCombatUi), nameof(NCombatUi.Activate), [typeof(CombatState)])];
    }

    public static void Postfix(NCombatUi __instance, CombatState state)
    {
        CurrentUi = __instance;
        var player = LocalContext.GetMe(state);
        if (player is not { Character: FighterCharacter }) return;

        // Frame counter — same spot as Regent's StarCounter (bottom, anchored top=1.0)
        var counter = __instance.GetNodeOrNull<FighterFrameCounter>("FighterFrameCounter");
        if (counter == null)
        {
            counter = new FighterFrameCounter { Name = "FighterFrameCounter" };
            counter.AnchorLeft = 0f; counter.AnchorTop = 1f;
            counter.AnchorRight = 0f; counter.AnchorBottom = 1f;
            counter.OffsetLeft = 64; counter.OffsetTop = -212;
            counter.OffsetRight = 192; counter.OffsetBottom = -84;
            counter.MouseFilter = Control.MouseFilterEnum.Ignore;
            counter.ZIndex = 100;
            __instance.AddChild(counter);
            counter.Init();
        }
        counter.Visible = true;

        // Super Gauge bar — below frame counter
        var gauge = __instance.GetNodeOrNull<FighterSuperGauge>("FighterSuperGauge");
        if (gauge == null)
        {
            gauge = new FighterSuperGauge { Name = "FighterSuperGauge" };
            gauge.AnchorLeft = 0f; gauge.AnchorTop = 1f;
            gauge.AnchorRight = 0f; gauge.AnchorBottom = 1f;
            gauge.OffsetLeft = 64; gauge.OffsetTop = -80;
            gauge.OffsetRight = 192; gauge.OffsetBottom = -62;
            gauge.MouseFilter = Control.MouseFilterEnum.Ignore;
            gauge.ZIndex = 100;
            __instance.AddChild(gauge);
            gauge.Init();
        }
        gauge.Visible = true;

        // Fighting Spirit gauge — below super gauge
        var spirit = __instance.GetNodeOrNull<FightingSpiritGauge>("FightingSpiritGauge");
        if (spirit == null)
        {
            spirit = new FightingSpiritGauge { Name = "FightingSpiritGauge" };
            spirit.AnchorLeft = 0f; spirit.AnchorTop = 1f;
            spirit.AnchorRight = 0f; spirit.AnchorBottom = 1f;
            spirit.OffsetLeft = 64; spirit.OffsetTop = -56;
            spirit.OffsetRight = 192; spirit.OffsetBottom = -38;
            spirit.MouseFilter = Control.MouseFilterEnum.Ignore;
            spirit.ZIndex = 100;
            __instance.AddChild(spirit);
            spirit.Init();
        }
        spirit.Visible = true;

        Refresh(player);
    }

    /// <summary>Force refresh both gauge displays for the given player.</summary>
    public static void Refresh(Player player)
    {
        if (player.Character is not FighterCharacter) return;
        var ui = CurrentUi;
        if (ui == null || !GodotObject.IsInstanceValid(ui)) return;

        ui.GetNodeOrNull<FighterSuperGauge>("FighterSuperGauge")
            ?.SetGauge(SecondaryResourceCmd.Get(player, FighterResources.SuperGauge));

        ui.GetNodeOrNull<FighterFrameCounter>("FighterFrameCounter")
            ?.SetFrames(SecondaryResourceCmd.Get(player, FighterResources.FrameAdvantage));

        var spiritAmount = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        var spiritMax = SecondaryResourceCmd.GetMax(player, FighterResources.FightingSpirit) ?? 6;
        ui.GetNodeOrNull<FightingSpiritGauge>("FightingSpiritGauge")
            ?.SetSpirit(spiritAmount, spiritMax);
    }
}

public sealed class FighterCombatUiAnimOutPatch : IPatchMethod
{
    public static string PatchId => "fighter_combat_ui_anim_out";
    public static string Description => "Hide gauge nodes on AnimOut";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(NCombatUi), nameof(NCombatUi.AnimOut))];
    }

    public static void Postfix(NCombatUi __instance)
    {
        var gauge = __instance.GetNodeOrNull<FighterSuperGauge>("FighterSuperGauge");
        if (gauge != null) gauge.Visible = false;
        var counter = __instance.GetNodeOrNull<FighterFrameCounter>("FighterFrameCounter");
        if (counter != null) counter.Visible = false;
    }
}

public sealed class FighterCombatUiDeactivatePatch : IPatchMethod
{
    public static string PatchId => "fighter_combat_ui_deactivate";
    public static string Description => "Hide gauge nodes on Deactivate";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return [new(typeof(NCombatUi), nameof(NCombatUi.Deactivate))];
    }

    public static void Postfix(NCombatUi __instance)
    {
        var gauge = __instance.GetNodeOrNull<FighterSuperGauge>("FighterSuperGauge");
        if (gauge != null) gauge.Visible = false;
        var counter = __instance.GetNodeOrNull<FighterFrameCounter>("FighterFrameCounter");
        if (counter != null) counter.Visible = false;
    }
}
