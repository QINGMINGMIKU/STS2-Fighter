using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.SecondaryResources;
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

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [FighterKeywords.Combo!.CardKeywordValue];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/strike_h.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var combo = Owner!.Creature.GetPower<Combo>();
        if (combo != null && combo.Amount > 0)
        {
            // Consume frames
            await FrameHelper.Lose(Owner, ComboFrameCost);

            // Consume Combo (single stack only)
            await PowerCmd.Remove(combo);

            // TC power: draw on combo consume
            var tc = Owner.Creature.GetPower<TCPower>();
            Godot.GD.Print($"[Fighter] Strike_H TC check: tc={tc?.GetType().Name ?? "null"}");
            if (tc != null)
                await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}
