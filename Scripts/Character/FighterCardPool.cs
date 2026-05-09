using Godot;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

public sealed class FighterCardPool : TypeListCardPoolModel
{
    public override string Title => "格斗家";
    public override string EnergyColorName => "red";
    public override Color DeckEntryCardColor => new(0.85f, 0.12f, 0.08f);
    public override bool IsColorless => false;

    public override string? BigEnergyIconPath => "Fighter/images/ui/energy_fighter_big.png";
    public override string? TextEnergyIconPath => "Fighter/images/ui/energy_fighter.png";
}
