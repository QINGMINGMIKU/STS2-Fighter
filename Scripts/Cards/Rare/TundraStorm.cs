using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterCard(typeof(FighterCardPool))]
public sealed class TundraStorm : ModCardTemplate
{
    public TundraStorm() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
    }

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: "Fighter/images/card_portraits/tundra_storm.png"
    );

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bufferAmount = IsUpgraded ? 2 : 1;

        await PowerCmd.Apply<BufferPower>(choiceContext, Owner!.Creature, bufferAmount, Owner.Creature, this);

        var blockedDamage = CounterHitState.PlayerDamageBlockedThisTurn;
        if (blockedDamage > 0)
        {
            await CreatureCmd.Heal(Owner.Creature, blockedDamage);
            CounterHitState.PlayerDamageBlockedThisTurn = 0;
        }
    }
}
