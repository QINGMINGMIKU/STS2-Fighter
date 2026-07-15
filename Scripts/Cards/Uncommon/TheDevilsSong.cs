using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class TheDevilsSong : ModCardTemplate
{
    private const int SuperGaugeCost = 2;
    private const int StacksBase = 2;
    private const int StacksUpgrade = 1;

    public TheDevilsSong() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override IEnumerable<string> RegisteredKeywordIds => [
        FighterKeywords.SuperId, FighterKeywords.TipsyId
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Stacks", StacksBase)
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
        PortraitPath: "Fighter/images/card_portraits/the_devils_song.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CancelHelper.ConsumeCancel(choiceContext, Owner!.Creature);
        await SuperArtTalisman.SpendGauge(Owner!, SuperGaugeCost);
        await PowerCmd.Apply<DevilsSongPower>(choiceContext, Owner!.Creature,
            DynamicVars["Stacks"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(StacksUpgrade);
    }
}
