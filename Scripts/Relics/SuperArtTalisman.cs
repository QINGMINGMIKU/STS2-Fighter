using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterRelic(typeof(FighterRelicPool))]
[RegisterCharacterStarterRelic(typeof(FighterCharacter))]
public sealed class SuperArtTalisman : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public const int AttacksPerGauge = 3;
    public const int MaxGauge = 3;

    private int _attackCount;

    public override bool ShowCounter => true;
    public override int DisplayAmount => HasGauge(Owner!);

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/relics/super_art_talisman.png",
        IconOutlinePath: "Fighter/images/relics/super_art_talisman_outline.png",
        BigIconPath: "Fighter/images/relics/super_art_talisman_big.png"
    );

    public void IncrementAttackCounter()
    {
        if (Owner == null) return;

        _attackCount++;
        if (_attackCount >= AttacksPerGauge)
        {
            _attackCount = 0;
            _ = GainGauge(Owner, 1);
        }
    }

    public static int HasGauge(Player player)
    {
        return SecondaryResourceCmd.Get(player, FighterResources.SuperGauge);
    }

    public static bool HasGauge(Player player, int amount)
    {
        return HasGauge(player) >= amount;
    }

    public static async Task SpendGauge(Player player, int amount)
    {
        await SecondaryResourceCmd.Lose(player, FighterResources.SuperGauge, amount);

        foreach (var relic in player.Relics)
        {
            if (relic is SuperArtTalisman talisman)
                talisman.InvokeDisplayAmountChanged();
        }
    }

    public static async Task GainGauge(Player player, int amount)
    {
        await SecondaryResourceCmd.Gain(player, FighterResources.SuperGauge, amount);

        foreach (var relic in player.Relics)
        {
            if (relic is SuperArtTalisman talisman)
            {
                talisman.Flash();
                talisman.InvokeDisplayAmountChanged();
            }
        }
    }
}
