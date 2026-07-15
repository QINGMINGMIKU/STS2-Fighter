using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class TundraStorm : ModCardTemplate
{
    private const int BufferBase = 1;
    private const int BufferUpgrade = 1;

    public TundraStorm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        ModCardVars.Int("Buffer", BufferBase)
    ];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/tundra_storm.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var buf = DynamicVars["Buffer"].IntValue;
        await PowerCmd.Apply<BufferPower>(choiceContext, Owner!.Creature, buf, Owner.Creature, this);
        await PowerCmd.Apply<TundraStormPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Buffer"].UpgradeValueBy(BufferUpgrade);
    }
}
