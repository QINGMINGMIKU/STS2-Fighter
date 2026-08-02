using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib.Combat.SecondaryResources;

namespace Fighter;

/// <summary>
/// Container panel for all three Fighter combat UI gauges.
/// Registered via RitsuLib RegisterCombatUi — lifecycle and visibility are automatic.
/// </summary>
[Tool]
public partial class FighterGaugePanel : Control
{
    private FighterFrameCounter? _frameCounter;
    private FighterSuperGauge? _superGauge;
    private FightingSpiritGauge? _spiritGauge;
    private bool _initialized;

    public override void _Ready()
    {
        Init();
    }

    public void Init()
    {
        if (_initialized) return;
        _initialized = true;

        // Anchor to bottom-left, same as EnergyCounterContainer
        AnchorLeft = 0f;
        AnchorTop = 1f;
        AnchorRight = 0f;
        AnchorBottom = 1f;
        MouseFilter = MouseFilterEnum.Ignore;

        // ── Frame Counter ──
        _frameCounter = new FighterFrameCounter
        {
            Name = "FrameCounter",
            OffsetLeft = 64,
            OffsetTop = -212,
            OffsetRight = 192,
            OffsetBottom = -84,
        };
        AddChild(_frameCounter);
        _frameCounter.Init();

        // ── Super Gauge ──
        _superGauge = new FighterSuperGauge
        {
            Name = "SuperGauge",
            OffsetLeft = 96,
            OffsetTop = -80,
            OffsetRight = 224,
            OffsetBottom = -62,
        };
        AddChild(_superGauge);
        _superGauge.Init();

        // ── Fighting Spirit ──
        _spiritGauge = new FightingSpiritGauge
        {
            Name = "SpiritGauge",
            OffsetLeft = 96,
            OffsetTop = -56,
            OffsetRight = 224,
            OffsetBottom = -38,
        };
        AddChild(_spiritGauge);
        _spiritGauge.Init();
    }

    /// <summary>Called by RitsuLib combat UI updater each refresh.</summary>
    public void Refresh(Player player)
    {
        if (!_initialized) Init();

        _frameCounter?.SetFrames(SecondaryResourceCmd.Get(player, FighterResources.FrameAdvantage));
        _superGauge?.SetGauge(SecondaryResourceCmd.Get(player, FighterResources.SuperGauge));

        var spirit = SecondaryResourceCmd.Get(player, FighterResources.FightingSpirit);
        var spiritMax = SecondaryResourceCmd.GetMax(player, FighterResources.FightingSpirit) ?? 6;
        _spiritGauge?.SetSpirit(spirit, spiritMax);
    }
}
