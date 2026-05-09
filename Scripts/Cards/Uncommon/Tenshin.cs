using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class Tenshin : ModCardTemplate
{
    private const int FrameCost = 4;

    public Tenshin() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<string> RegisteredKeywordIds => [FighterKeywords.ThrowId];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/tenshin.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tipsy = GetEffectiveTipsy();
        if (tipsy >= 3)
        {
            await CancelHelper.ConsumeCancel(choiceContext, Owner!.Creature);
        }
        else
        {
            var fa = Owner!.Creature.GetPower<FrameAdvantage>();
            if (fa != null)
                await PowerCmd.ModifyAmount(choiceContext, fa, -FrameCost, Owner.Creature, null, false);
        }

        var vulnCount = tipsy >= 3 ? 3 : 2;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, vulnCount, Owner.Creature, this);

        if (tipsy >= 3)
            await PowerCmd.Apply<Cancel>(choiceContext, Owner!.Creature, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        // Upgrade: 给予3层易伤 (from 2), cost reduction already handled by tipsy check
    }

    private int GetEffectiveTipsy() => TipsyHelper.GetEffectiveTipsy(Owner!.Creature);
}
