using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterPower]
public sealed class ConfirmPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/confirm.png",
        BigIconPath: "Fighter/images/powers/confirm_big.png"
    );

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null || dealer != Owner)
            return 1m;

        if (IsComboOrCancelCard(cardSource))
            return 1.5m;

        return 1m;
    }

    private static bool IsComboOrCancelCard(CardModel card)
    {
        foreach (var kw in card.Keywords)
        {
            if (FighterKeywords.Combo != null && kw == FighterKeywords.Combo.CardKeywordValue)
                return true;
            if (FighterKeywords.Cancel != null && kw == FighterKeywords.Cancel.CardKeywordValue)
                return true;
        }
        return false;
    }
}
