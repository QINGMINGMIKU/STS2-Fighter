using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterPower]
public sealed class Tipsy : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/tipsy.png",
        BigIconPath: "Fighter/images/powers/tipsy_big.png"
    );

    /// <summary>
    /// Damage multiplier for cards with the Tipsy keyword.
    /// Scales from 0.90 (0 Tipsy) to 1.10 (4 Tipsy).
    /// </summary>
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay = null)
    {
        if (cardSource == null || dealer != Owner)
            return 1m;

        if (!HasTipsyKeyword(cardSource))
            return 1m;

        var level = TipsyHelper.GetEffectiveTipsy(Owner!);
        return 0.90m + (level * 0.05m);
    }

    private static bool HasTipsyKeyword(CardModel card)
    {
        if (FighterKeywords.Tipsy == null) return false;
        return card.Keywords.Contains(FighterKeywords.Tipsy.CardKeywordValue);
    }
}
