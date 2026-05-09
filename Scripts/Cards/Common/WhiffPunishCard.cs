using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class WhiffPunishCard : ModCardTemplate
{
    public WhiffPunishCard() : base(1, CardType.Power, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/whiff_punish.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<WhiffPunish>(choiceContext, Owner!.Creature, 1, Owner.Creature, this);
    }
}
