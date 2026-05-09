using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class YiJianlianCard : ModCardTemplate
{
    public YiJianlianCard() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/yi_jianlian.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<YiJianlianPower>(choiceContext, Owner!.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.SetCustomBaseCost(1);
    }
}
