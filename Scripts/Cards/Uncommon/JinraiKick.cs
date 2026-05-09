using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class JinraiKick : ModCardTemplate
{
    private const int ComboDiscount = 1;

    public JinraiKick() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move)
    ];

    protected override IEnumerable<string> RegisteredCardTagIds => ["Strike"];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/jinrai_kick.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hasCombo = Owner!.Creature.GetPower<Combo>() is { Amount: > 0 };
        var bonus = hasCombo ? 1m : 0m;

        for (var i = 0; i < 3; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus)
                .FromCard(this)
                .Targeting(cardPlay.Target!)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}
