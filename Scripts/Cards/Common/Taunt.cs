using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class Taunt : ModCardTemplate
{
    private const int FrameLoss = 5;

    public Taunt() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/taunt.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var fa = Owner!.Creature.GetPower<FrameAdvantage>();
        if (fa == null)
        {
            await PowerCmd.Apply<FrameAdvantage>(choiceContext, Owner.Creature, 0, Owner.Creature, this);
            fa = Owner.Creature.GetPower<FrameAdvantage>();
        }

        if (fa != null)
            await PowerCmd.ModifyAmount(choiceContext, fa, -FrameLoss, Owner.Creature, null, false);

        var strGain = IsUpgraded ? 3 : 2;
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, strGain, Owner.Creature, this);
    }
}
