using Godot;

namespace Fighter;

/// <summary>
/// Fighting Spirit gauge — single bar with gradient color.
/// 6 = green, 3 = yellow, 0 = gray.
/// </summary>
[Tool]
public partial class FightingSpiritGauge : Control
{
    private ColorRect? _bar;
    private Label? _label;
    private bool _initialized;
    private int _displayed = -1;

    private static readonly Color SpiritGreen = new(0.1f, 0.85f, 0.2f, 1f);
    private static readonly Color SpiritYellow = new(1f, 0.75f, 0.1f, 1f);
    private static readonly Color SpiritGray = new(0.3f, 0.3f, 0.3f, 1f);

    public override void _Ready() => Init();

    public void Init()
    {
        if (_initialized) return;
        _initialized = true;

        // Background
        var bg = new ColorRect
        {
            Color = new Color(0.1f, 0.1f, 0.1f, 0.6f),
            MouseFilter = MouseFilterEnum.Ignore,
        };
        bg.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(bg);

        // Fill bar
        _bar = new ColorRect
        {
            Color = SpiritGreen,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        _bar.SetAnchorsPreset(LayoutPreset.CenterLeft);
        _bar.AnchorRight = 1f;
        _bar.AnchorBottom = 1f;
        _bar.SizeFlagsHorizontal = SizeFlags.Fill;
        AddChild(_bar);

        // Label
        _label = new Label
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        _label.SetAnchorsPreset(LayoutPreset.FullRect);
        _label.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));
        _label.AddThemeFontSizeOverride("font_size", 14);
        AddChild(_label);
    }

    public void SetSpirit(int amount, int max)
    {
        if (amount == _displayed) return;
        _displayed = amount;

        var ratio = max > 0 ? (float)amount / max : 0f;

        // Color: lerp gray → yellow → green
        Color targetColor;
        if (ratio <= 0f)
            targetColor = SpiritGray;
        else if (ratio <= 0.5f)
            targetColor = SpiritGray.Lerp(SpiritYellow, ratio * 2f);
        else
            targetColor = SpiritYellow.Lerp(SpiritGreen, (ratio - 0.5f) * 2f);

        // Animate fill width
        if (_bar != null)
        {
            var tween = CreateTween();
            tween.TweenProperty(_bar, "color", targetColor, 0.2f);
            tween.Parallel().TweenMethod(Callable.From<float>(v =>
            {
                _bar.AnchorRight = 1f - v;
            }), 1f - ratio, 1f - ratio, 0.2f);
        }

        if (_label != null)
        {
            _label.Text = $"{amount}/{max}";
            _label.AddThemeColorOverride("font_color", amount > 0
                ? new Color(1, 1, 1, 1)
                : new Color(0.5f, 0.5f, 0.5f, 1));
        }
    }
}
