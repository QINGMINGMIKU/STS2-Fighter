using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

/// <summary>
/// 确反康标记 — applied to enemies whose attack was fully blocked.
/// Player deals +20% damage and gains +4 frames on next hit, then mark expires.
/// </summary>
[RegisterPower]
public sealed class PunishCounterPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public const float DamageMultiplier = 1.20f;
    public const int BonusFrames = 4;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/punish_counter.png",
        BigIconPath: "Fighter/images/powers/punish_counter_big.png"
    );
}
