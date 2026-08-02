using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class SiberianExpress : ModCardTemplate
{
    private const int BaseGauge = 1;
    private const int CounterHitGauge = 2;

    public SiberianExpress() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(14, ValueProp.Move)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [FighterKeywords.Throw!.CardKeywordValue];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/siberian_express.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        var target = cardPlay.Target!;
        var isCounterHit = target.IsMonster
            && target.Monster?.NextMove != null
            && target.Monster.NextMove.Intents?.OfType<AttackIntent>().Any() == true;

        var gain = isCounterHit ? CounterHitGauge : BaseGauge;
        await SuperArtTalisman.GainGauge(Owner!, gain);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
