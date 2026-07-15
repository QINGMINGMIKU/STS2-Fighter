using Godot;

namespace Fighter;

/// <summary>
/// Street Fighter 6 style Super Gauge — 3-segment bar with tween animation.
/// Call SetGauge(amount) to animate to target value.
/// </summary>
[Tool]
public partial class FighterSuperGauge : Control
{
    private const int MaxSegments = 3;
    private static readonly Color EmptyColor = new(0.12f, 0.12f, 0.12f, 1f);
    private static readonly Color FillColor = new(1f, 0.82f, 0.0f, 1f);
    private static readonly Color FlashColor = new(1f, 0.95f, 0.4f, 1f);

    private readonly ColorRect[] _segments = new ColorRect[MaxSegments];
    private int _displayed = -1;
    private bool _initialized;

    public override void _Ready()
    {
        Init();
    }

    /// <summary>Call immediately after AddChild — before _Ready fires.</summary>
    public void Init()
    {
        if (_initialized) return;
        _initialized = true;

        var container = new HBoxContainer();
        container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        container.AddThemeConstantOverride("separation", 4);
        AddChild(container);

        for (var i = 0; i < MaxSegments; i++)
        {
            var segment = new ColorRect
            {
                Name = $"Seg{i}",
                CustomMinimumSize = new Vector2(28, 12),
                Color = EmptyColor,
                SizeFlagsVertical = SizeFlags.ShrinkCenter,
            };
            container.AddChild(segment);
            _segments[i] = segment;
        }

    }

    public void SetGauge(int amount)
    {
        amount = Math.Clamp(amount, 0, MaxSegments);
        if (amount == _displayed) return;
        _displayed = amount;

        for (var i = 0; i < MaxSegments; i++)
        {
            var filled = i < amount;
            var target = filled ? FillColor : EmptyColor;
            if (_segments[i].Color == target) continue;

            var tween = CreateTween();
            tween.TweenProperty(_segments[i], "color", target, 0.15f);
        }

        // Flash on max
        if (amount >= MaxSegments)
        {
            foreach (var seg in _segments)
            {
                var flash = CreateTween().SetLoops(2);
                flash.TweenProperty(seg, "color", FlashColor, 0.18f);
                flash.TweenProperty(seg, "color", FillColor, 0.42f);
            }
        }
    }
}
