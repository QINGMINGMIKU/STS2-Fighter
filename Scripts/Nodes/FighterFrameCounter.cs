using Godot;
using MegaCrit.Sts2.addons.mega_text;

namespace Fighter;

/// <summary>
/// Frame Advantage counter — shows current frame count with color coding.
/// Positive = blue, Negative = red.
/// </summary>
[Tool]
public partial class FighterFrameCounter : Control
{
    private static readonly Color PositiveColor = new(0.2f, 0.6f, 1f, 1f);
    private static readonly Color NegativeColor = new(1f, 0.2f, 0.2f, 1f);
    private static readonly Color ZeroColor = new(0.6f, 0.6f, 0.6f, 1f);

    private MegaLabel? _label;
    private TextureRect? _icon;
    private int _displayed = int.MinValue;
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

        // Star background placeholder (same as Regent's StarCounter)
        if (ResourceLoader.Exists("res://images/ui/combat/energy_star.png"))
        {
            var bg = new TextureRect
            {
                Name = "StarBg",
                Texture = ResourceLoader.Load<Texture2D>("res://images/ui/combat/energy_star.png"),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                MouseFilter = MouseFilterEnum.Ignore,
                Modulate = new Color(1, 1, 1, 0.5f),
            };
            bg.SetAnchorsPreset(LayoutPreset.FullRect);
            AddChild(bg);
        }

        var container = new HBoxContainer();
        container.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        container.AddThemeConstantOverride("separation", 4);
        AddChild(container);

        _label = new MegaLabel
        {
            Name = "FrameLabel",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            MinFontSize = 36,
            MaxFontSize = 36,
        };
        container.AddChild(_label);
    }

    private void TryLoadIcon(string path)
    {
        if (ResourceLoader.Exists(path))
            _icon!.Texture = ResourceLoader.Load<Texture2D>(path);
    }

    public void SetIcon(string path)
    {
        if (ResourceLoader.Exists(path))
            _icon!.Texture = ResourceLoader.Load<Texture2D>(path);
    }

    public void SetFrames(int amount)
    {
        if (amount == _displayed) return;

        var prev = _displayed;
        _displayed = amount;

        _label!.Text = $"{amount}";

        if (amount > 0)
            _label.Modulate = PositiveColor;
        else if (amount < 0)
            _label.Modulate = NegativeColor;
        else
            _label.Modulate = ZeroColor;

        // Pulse on change
        if (prev != int.MinValue)
        {
            var tween = CreateTween();
            tween.TweenProperty(_label, "scale", new Vector2(1.3f, 1.3f), 0.1f);
            tween.TweenProperty(_label, "scale", Vector2.One, 0.15f);
        }
    }
}
