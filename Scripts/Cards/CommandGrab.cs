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
public sealed class CommandGrab : ModCardTemplate
{
    private const int FrameCost = 6;

    public CommandGrab() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(13, ValueProp.Move | ValueProp.Unblockable)
    ];

    protected override IEnumerable<string> RegisteredCardTagIds => ["Strike"];

    protected override IEnumerable<string> RegisteredKeywordIds => [FighterKeywords.ThrowId];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/command_grab.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discount = Owner!.Creature.GetPower<StrikeThrowMixup>() != null ? 3 : 0;
        var effectiveCost = Math.Max(0, FrameCost - discount);

        var fa = Owner!.Creature.GetPower<FrameAdvantage>();
        if (fa != null)
            await PowerCmd.ModifyAmount(choiceContext, fa, -effectiveCost, Owner.Creature, null, false);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
