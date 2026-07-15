using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
[RegisterCharacterStarterCard(typeof(FighterCharacter), 1)]
public sealed class DriveRush : ModCardTemplate
{
    private const int SpiritCost = 2;
    private const int FrameGain = 4;
    private const int FrameGainUpgrade = 2;

    public DriveRush() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Frames", FrameGain)
    ];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            if (Owner == null) return false;
            return SecondaryResourceCmd.Get(Owner, FighterResources.FightingSpirit) > 0;
        }
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/drive_rush.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sufficient = await SpiritHelper.SpendSpirit(Owner!, SpiritCost);
        var frames = sufficient
            ? DynamicVars["Frames"].IntValue
            : (int)(DynamicVars["Frames"].IntValue * SpiritHelper.InsufficientPenalty);

        if (frames > 0)
            await FrameHelper.Gain(Owner!, frames);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Frames"].UpgradeValueBy(FrameGainUpgrade);
    }
}
