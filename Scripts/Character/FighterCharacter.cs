using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Characters;

namespace Fighter;

[RegisterCharacter]
public sealed class FighterCharacter : ModCharacterTemplate<FighterCardPool, FighterRelicPool, FighterPotionPool>
{
    public override bool RequiresEpochAndTimeline => false;

    public override Color MapDrawingColor => new(0.85f, 0.12f, 0.08f);
    public override Color EnergyLabelOutlineColor => new(0xFF4A1A22);
    public override Color NameColor => new(0.85f, 0.12f, 0.08f);
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;
    public override int StartingGold => 99;
    public override float AttackAnimDelay => 0.15f;
    public override float CastAnimDelay => 0.25f;

    public override List<string> GetArchitectAttackVfx() =>
    [
        "vfx/vfx_attack_blunt", "vfx/vfx_heavy_blunt", "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact", "vfx/vfx_rock_shatter"
    ];
}
