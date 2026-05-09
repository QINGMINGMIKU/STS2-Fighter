using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class DoubleLariat : ModCardTemplate
{
    private const int CancelStacks = 2;
    private const int FrameThreshold = 5;

    public DoubleLariat() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move)
    ];

    protected override IEnumerable<string> RegisteredKeywordIds => [
        FighterKeywords.CancelId, FighterKeywords.SpecialId
    ];

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable) return false;
            return SpecialHelper.CanPlaySpecial(this, Owner!);
        }
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/double_lariat.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Cancel: consume existing for discount, then apply new
        var hadCancel = await CancelHelper.ConsumeCancel(choiceContext, Owner!.Creature);
        if (hadCancel)
            await PlayerCmd.GainEnergy(1, Owner);

        // Special cost
        await SpecialHelper.PaySpecialCost(choiceContext, Owner, this);

        await PowerCmd.Apply<Cancel>(choiceContext, Owner.Creature, CancelStacks, Owner.Creature, this);

        // AoE 3 damage twice to all enemies
        if (CombatState == null) return;
        var enemies = CombatState.GetOpponentsOf(Owner.Creature).ToList();
        for (var hit = 0; hit < 2; hit++)
        {
            foreach (var enemy in enemies)
            {
                await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
                    .FromCard(this)
                    .Targeting(enemy)
                    .Execute(choiceContext);
            }
        }

        // Draw 2, consume 5 frames (can go negative)
        await PowerCmd.ModifyAmount(choiceContext,
            Owner!.Creature.GetPower<FrameAdvantage>()!, -FrameThreshold, Owner.Creature, null);
        await CardPileCmd.Draw(choiceContext, 2, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }
}
