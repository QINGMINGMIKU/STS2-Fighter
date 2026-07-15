using System.Collections.Generic;
using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

/// <summary>
/// While active, blocked damage heals you and is reflected back to the attacker.
/// Expires after one turn (following ReflectPower's pattern).
/// </summary>
[RegisterPower]
public sealed class TundraStormPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/powers/tundra_storm.png",
        BigIconPath: "Fighter/images/powers/tundra_storm_big.png"
    );

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.BlockedDamage <= 0 || dealer == null)
            return;

        // Heal the blocked amount
        await CreatureCmd.Heal(target, result.BlockedDamage);

        // Reflect the blocked damage back to the attacker
        if (props.IsPoweredAttack())
            await CreatureCmd.Damage(choiceContext, dealer, result.BlockedDamage, ValueProp.Unpowered, null, null);
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;

        if (Amount <= 1)
            await PowerCmd.Remove(this);
        else
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -1, Owner, null);
    }
}
