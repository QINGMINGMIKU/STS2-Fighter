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
public sealed class GetsugaSaiho : ModCardTemplate
{
    private const int SuperGaugeCost = 3;

    public GetsugaSaiho() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(35, ValueProp.Move)
    ];

    protected override IEnumerable<string> RegisteredKeywordIds => [
        FighterKeywords.SuperId
    ];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            return SuperArtTalisman.HasGauge(Owner!, SuperGaugeCost);
        }
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/getsuga_saiho.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CancelHelper.ConsumeCancel(choiceContext, Owner!.Creature);
        await SuperArtTalisman.SpendGauge(Owner!, SuperGaugeCost);

        var tipsy = GetEffectiveTipsy();
        var damage = DynamicVars.Damage.BaseValue;
        if (tipsy == 4)
            damage *= 2;

        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        if (tipsy == 4)
            await PowerCmd.Apply<SuperGauge>(choiceContext, Owner.Creature, SuperGaugeCost, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(15);
    }

    private int GetEffectiveTipsy() => TipsyHelper.GetEffectiveTipsy(Owner!.Creature);
}
