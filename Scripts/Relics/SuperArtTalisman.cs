using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace Fighter;

[RegisterRelic(typeof(FighterRelicPool))]
[RegisterCharacterStarterRelic(typeof(FighterCharacter))]
public sealed class SuperArtTalisman : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const int AttacksPerGauge = 3;
    private const int MaxGauge = 3;

    private int _attackCount;
    private int _gaugeCount;

    public override bool ShowCounter => true;
    public override int DisplayAmount => _gaugeCount;

    public override RelicAssetProfile AssetProfile => new(
        IconPath: "Fighter/images/relics/super_art_talisman.png",
        IconOutlinePath: "Fighter/images/relics/super_art_talisman_outline.png",
        BigIconPath: "Fighter/images/relics/super_art_talisman_big.png"
    );

    public void IncrementAttackCounter()
    {
        _attackCount++;
        if (_attackCount >= AttacksPerGauge)
        {
            _attackCount = 0;
            if (_gaugeCount < MaxGauge)
            {
                _gaugeCount++;
                Flash();
                InvokeDisplayAmountChanged();
            }
        }
    }

    public static bool HasGauge(Player player, int amount)
    {
        foreach (var relic in player.Relics)
        {
            if (relic is SuperArtTalisman talisman)
                return talisman._gaugeCount >= amount;
        }
        return false;
    }

    public static Task SpendGauge(Player player, int amount)
    {
        foreach (var relic in player.Relics)
        {
            if (relic is SuperArtTalisman talisman && talisman._gaugeCount >= amount)
            {
                talisman._gaugeCount -= amount;
                talisman.InvokeDisplayAmountChanged();
                return Task.CompletedTask;
            }
        }
        return Task.CompletedTask;
    }
}
