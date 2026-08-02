using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class GrandStorm : ModCardTemplate
{
    private const int SuperGaugeCost = 3;

    public GrandStorm() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(50, ValueProp.Move | ValueProp.Unblockable)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        FighterKeywords.Throw!.CardKeywordValue, FighterKeywords.Super!.CardKeywordValue
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
        PortraitPath: "Fighter/images/card_portraits/grand_storm.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Cancel discount
        var hadCancel = await CancelHelper.ConsumeCancel(choiceContext, Owner!.Creature);
        if (hadCancel)
            await PlayerCmd.GainEnergy(1, Owner);

        // Super: consume 3 Super Gauge
        await SuperArtTalisman.SpendGauge(Owner!, SuperGaugeCost);

        // Super: restore 3 spirit
        await SecondaryResourceCmd.Gain(Owner!, FighterResources.FightingSpirit, 3);

        // Consume all frames for bonus
        var frames = FrameHelper.Get(Owner!);
        var frameBonus = 0m;
        if (frames > 0)
        {
            frameBonus = frames;
            await FrameHelper.Lose(Owner!, frames);
        }

        var mainDamage = DynamicVars.Damage.BaseValue + frameBonus;
        var splashDamage = (DynamicVars.Damage.BaseValue + frameBonus) / 2;

        // Main target: full damage
        await DamageCmd.Attack(mainDamage)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);

        // Other enemies: half damage
        if (CombatState == null) return;
        var opponents = CombatState.GetOpponentsOf(Owner.Creature);
        foreach (var enemy in opponents)
        {
            if (enemy != cardPlay.Target)
            {
                await DamageCmd.Attack(splashDamage)
                    .FromCard(this, cardPlay)
                    .Targeting(enemy)
                    .Execute(choiceContext);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10);
    }
}
