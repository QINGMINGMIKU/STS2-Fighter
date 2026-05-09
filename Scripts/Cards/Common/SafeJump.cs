using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class SafeJump : ModCardTemplate
{
    public SafeJump() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/safe_jump.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FrameAdvantage>(choiceContext, Owner!.Creature, 3, Owner.Creature, this);
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.SetCustomBaseCost(1);
    }
}
