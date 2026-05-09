using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Fighter;

public static class SpecialHelper
{
    public const int SpiritCost = 2;

    public static bool CanPlaySpecial(CardModel card, Player player)
    {
        var spirit = player.Creature.GetPower<FightingSpirit>();
        if (spirit != null && spirit.Amount >= SpiritCost)
            return true;

        var energyCost = card.EnergyCost.Canonical;
        var currentEnergy = player.PlayerCombatState?.Energy ?? 0;
        if (currentEnergy < energyCost)
            return false;

        var frames = player.Creature.GetPower<FrameAdvantage>()?.Amount ?? 0;
        return frames >= energyCost * 2;
    }

    public static async Task<int> PaySpecialCost(PlayerChoiceContext ctx, Player player, CardModel card)
    {
        var spirit = player.Creature.GetPower<FightingSpirit>();
        if (spirit != null && spirit.Amount >= SpiritCost)
        {
            await SpiritHelper.SpendSpirit(ctx, player, SpiritCost);
            return 0;
        }

        var energyCost = card.EnergyCost.Canonical;
        var fa = player.Creature.GetPower<FrameAdvantage>();
        if (fa != null && fa.Amount >= energyCost * 2)
            await PowerCmd.ModifyAmount(ctx, fa, -(energyCost * 2), player.Creature, null, false);

        return energyCost;
    }
}
