using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

/// <summary>
/// 打康标记 — applied to enemies that intend to Attack.
/// Player deals +20% damage and gains +2 frames on next hit, then mark expires.
/// </summary>
[RegisterPower]
public sealed class CounterHitPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public const float DamageMultiplier = 1.20f;
    public const int BonusFrames = 2;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/counter_hit.png",
        BigIconPath: "Fighter/images/powers/counter_hit_big.png"
    );
}
