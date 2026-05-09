using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class TheDevilInside : ModCardTemplate
{
    public TheDevilInside() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    public override bool GainsBlock => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5, ValueProp.Move)
    ];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/the_devil_inside.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tipsy = Owner!.Creature.GetPower<Tipsy>();
        if (tipsy == null || tipsy.Amount < 4)
            await PowerCmd.Apply<Tipsy>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        if (TurnState.StarterPlayedThisTurn)
            await PowerCmd.Apply<FrameAdvantage>(choiceContext, Owner.Creature, 3, Owner.Creature, this);

        await CreatureCmd.GainBlock(Owner!.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
