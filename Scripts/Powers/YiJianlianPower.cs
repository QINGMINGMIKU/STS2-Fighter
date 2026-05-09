using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterPower]
public sealed class YiJianlianPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/yi_jianlian.png",
        BigIconPath: "Fighter/images/powers/yi_jianlian_big.png"
    );

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null || dealer != Owner)
            return 1m;

        if (HasCancel(cardSource))
            return 0.8m;

        return 1m;
    }

    public static bool IsCancelCard(CardModel card)
    {
        if (FighterKeywords.Cancel == null) return false;
        return card.Keywords.Contains(FighterKeywords.Cancel.CardKeywordValue);
    }

    private static bool HasCancel(CardModel card) => IsCancelCard(card);
}
