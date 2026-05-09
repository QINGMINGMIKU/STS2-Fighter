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
public sealed class SpiralArrow : ModCardTemplate
{
    public SpiralArrow() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override IEnumerable<string> RegisteredCardTagIds => ["Strike"];
    protected override IEnumerable<string> RegisteredKeywordIds => [FighterKeywords.StarterId];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/spiral_arrow.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        TurnState.StarterPlayedThisTurn = true;
        var hadCombo = Owner!.Creature.GetPower<Combo>() is { Amount: > 0 };

        await PowerCmd.Apply<Combo>(choiceContext, Owner.Creature, 1, Owner.Creature, this);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        if (hadCombo)
        {
            await PowerCmd.Apply<FrameAdvantage>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
