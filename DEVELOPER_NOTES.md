# Fighter Mod — 开发笔记

## 已验证可用的 API 签名 (用户游戏版本)

```csharp
// PowerCmd — 需要 choiceContext 第一参数
PowerCmd.Apply<T>(choiceContext, target, amount, applier, source);
PowerCmd.ModifyAmount(choiceContext, power, delta, applier, source, false); // 6参数
PowerCmd.Remove(power);       // 传 PowerModel 实例
PowerCmd.Remove<T>(creature); // 传 Creature

// CreatureCmd / PlayerCmd / CardPileCmd — 不需要 choiceContext
CreatureCmd.GainBlock(creature, blockVar, cardPlay);   // 注意: 无 choiceContext
CreatureCmd.Heal(creature, amount);                     // 无 choiceContext
PlayerCmd.GainEnergy(amount, player);                   // 无 choiceContext
CardPileCmd.Draw(choiceContext, count, player);         // 需要 choiceContext!

// DamageCmd — builder pattern
DamageCmd.Attack(value).FromCard(card).Targeting(target).Execute(choiceContext);
```

## 已验证可用的 Hook (Relic)

```csharp
// 可用
AfterCardPlayed(PlayerChoiceContext, CardPlay)   // 每张牌打出后
AfterPlayerTurnStart(PlayerChoiceContext, Player) // 回合开始
AfterDamageReceived(PlayerChoiceContext, Creature, DamageResult, ValueProp, Creature?, CardModel?)
ModifyDamageMultiplicative(Creature?, decimal, ValueProp, Creature?, CardModel?)
ModifyDamageAdditive(Creature?, decimal, ValueProp, Creature?, CardModel?)

// 不可用
AfterAttack   // 用户版本不可重写 (CS0115)
BeforeHandDraw // 参数含 ICombatState, 用户版本无此类型 (CS0246)
```

## 已验证可用的 Hook (Power)

```csharp
ModifyDamageMultiplicative  // 可用
ModifyDamageAdditive        // 可用 (StrikeThrowMixup用)
// Power 没有 AfterCardPlayed/AfterAttack
```

## 关键类命名空间

```csharp
using MegaCrit.Sts2.Core.Commands;           // PowerCmd, CreatureCmd, PlayerCmd, CardPileCmd, DamageCmd
using MegaCrit.Sts2.Core.Commands.Builders;   // AttackCommand
using MegaCrit.Sts2.Core.Entities.Cards;      // CardModel, CardTag, CardType, CardRarity, CardEnergyCost
using MegaCrit.Sts2.Core.Entities.Creatures;  // Creature, DamageResult
using MegaCrit.Sts2.Core.Entities.Players;    // Player
using MegaCrit.Sts2.Core.Entities.Powers;     // PowerModel, PowerType, PowerStackType
using MegaCrit.Sts2.Core.Entities.Relics;     // RelicModel, RelicRarity
using MegaCrit.Sts2.Core.GameActions.Multiplayer; // PlayerChoiceContext, CardPlay
using MegaCrit.Sts2.Core.Localization.DynamicVars; // DynamicVar, DamageVar, BlockVar, DynamicVarSet
using MegaCrit.Sts2.Core.Models;              // CardPoolModel, RelicPoolModel, PotionPoolModel, ModelDb
using MegaCrit.Sts2.Core.Models.Powers;       // StrengthPower, DexterityPower, WeakPower, VulnerablePower, BufferPower
using MegaCrit.Sts2.Core.MonsterMoves.Intents; // AttackIntent
using MegaCrit.Sts2.Core.ValueProps;          // ValueProp
using Godot;                                  // Color
using STS2RitsuLib;                           // RitsuLibFramework
using STS2RitsuLib.Cards.DynamicVars;         // ModCardVars
using STS2RitsuLib.Interop;                   // ModTypeDiscoveryHub
using STS2RitsuLib.Interop.AutoRegistration;  // [RegisterCard], [RegisterPower], etc.
using STS2RitsuLib.Keywords;                  // ModKeywordRegistry, ModKeywordDefinition
using STS2RitsuLib.Scaffolding.Content;       // ModCardTemplate, ModRelicTemplate, ModPowerTemplate, TypeList*PoolModel
using STS2RitsuLib.Scaffolding.Characters;    // ModCharacterTemplate
```

## 已验证的模板模式

### 卡牌 (ModCardTemplate)
```csharp
[RegisterCard(typeof(FighterCardPool))]
[RegisterCharacterStarterCard(typeof(FighterCharacter), count)]
public sealed class CardName : ModCardTemplate
{
    public CardName() : base(cost, CardType.Xxx, CardRarity.Xxx, TargetType.Xxx) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar/BlockVar(value, ValueProp.Move)];
    protected override IEnumerable<string> RegisteredCardTagIds => ["Strike"/"Defend"];
    protected override IEnumerable<string> RegisteredKeywordIds => [FighterKeywords.XxxId];

    public override CardAssetProfile AssetProfile => new(PortraitPath: "Fighter/images/card_portraits/name.png");

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cp) { ... }
    protected override void OnUpgrade() { ... }
}
```

### Power (ModPowerTemplate)
```csharp
[RegisterPower]
public sealed class PowerName : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff/Debuff;
    public override PowerStackType StackType => PowerStackType.Counter/Single;
    public override bool AllowNegative => true;  // 帧数专用
}
```

### Relic (ModRelicTemplate)
```csharp
[RegisterRelic(typeof(FighterRelicPool))]
[RegisterCharacterStarterRelic(typeof(FighterCharacter))]
public sealed class RelicName : ModRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool ShowCounter => true;
    public override int DisplayAmount => _value;
    // 计数器刷新: InvokeDisplayAmountChanged();
}
```

### Pool
```csharp
public sealed class FighterCardPool : TypeListCardPoolModel
{
    public override string Title => "...";
    public override string EnergyColorName => "red";  // 必须已存在的颜色名
    public override Color DeckEntryCardColor => new(r, g, b);
    public override bool IsColorless => false;
    public override string? BigEnergyIconPath => "...";
    public override string? TextEnergyIconPath => "...";
}
```

### 能量费用升级
```csharp
protected override void OnUpgrade() { EnergyCost.SetCustomBaseCost(newCost); }
```

## 已验证的文件结构

```
Scripts/
  Entry.cs                   ← [ModInitializer], RegisterModAssembly
  Cards/
    {CardName}.cs            ← 跨稀有度卡牌
    Basic/                   ← 掉落到这里也行
    Common/
    Uncommon/
    Rare/
  Powers/{PowerName}.cs
  Relics/{RelicName}.cs
  Keywords/FighterKeywords.cs
  Mechanics/CounterHitState.cs, SpiritHelper.cs, SpecialHelper.cs, CancelHelper.cs
  Character/FighterCharacter.cs, FighterCardPool.cs, FighterRelicPool.cs, FighterPotionPool.cs
Fighter/localization/{zhs,eng}/cards.json, relics.json, powers.json, characters.json, card_keywords.json
```

## 已验证的 JSON 格式 (扁平 key)
```json
{ "FIGHTER_CARD_CLASSNAME.title": "...", "FIGHTER_CARD_CLASSNAME.description": "..." }
```

## 陷阱 / 避坑

1. **csproj SDK 版本**: 必须匹配 GodotExe 版本 (用户用 4.5.1)
2. **PCK 导出**: `BasicExport` 预设名, Godot 4.5.1 导出
3. **EnergyColorName**: 不能用中文, 游戏找不到图标 (用 "red" 等已有颜色)
4. **CardPileCmd.Draw**: 需要 choiceContext! 其他 Cmd 不需要
5. **PowerCmd.ModifyAmount**: 需要6个参数, 最后一个是 bool silent
6. **PowerCmd.Remove**: 传 PowerModel 实例, 不要传 choiceContext
7. **Power.Owner**: 就是 Creature, 不是 Player (不用 Owner?.Creature)
8. **RitsuLib ID 格式**: `{MODID}_{CATEGORY}_{TYPE_NAME}` 全大写
9. **Dotnet 不在 Docker 环境**: 只能检查代码, 构建在 Windows 上进行
10. **ICombatState 不存在于用户版本**: 用 AfterPlayerTurnStart 替代 BeforeHandDraw
11. **AfterAttack 不可重写于用户版本**: 用 AfterCardPlayed 替代
12. **Relic 的 AfterCardPlayed 可能不触發**: 迁到 FighterHeadband (已验证可行)
13. **CombatState.GetOpponentsOf(creature)** 获取敌人列表
14. **DamageResult.TotalDamage** 获取伤害总量
