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
[RegisterCharacterStarterCard(typeof(FighterCharacter), 1)]
public sealed class Strike_H : ModCardTemplate
{
    private const int ComboFrameCost = 3;

    public Strike_H() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move)
    ];

    protected override IEnumerable<string> RegisteredCardTagIds => ["Strike"];
    protected override IEnumerable<string> RegisteredKeywordIds => [FighterKeywords.ComboId];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/strike_h.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combo = Owner!.Creature.GetPower<Combo>();
        if (combo != null && combo.Amount > 0)
        {
            var fa = Owner.Creature.GetPower<FrameAdvantage>();
            if (fa != null)
                await PowerCmd.ModifyAmount(choiceContext, fa, -ComboFrameCost, Owner.Creature, null, false);

            await PowerCmd.Remove<Combo>(Owner.Creature);

            var tc = Owner.Creature.GetPower<TCPower>();
            if (tc != null)
                await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
