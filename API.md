# STS2 Modding API Reference

Quick lookups for STS2 mod development. Built from BaseLib source, STS2-resource directory structure, QuickReload reference mod, and Fighter mod compilation experience.

## Build

```cmd
dotnet build Fighter.csproj -c Release
```

Always build from `.csproj`, not `.sln`.

## Namespace Map

| Type | Namespace | Source |
|------|-----------|--------|
| `PowerType` enum | `MegaCrit.Sts2.Core.Entities.Powers` | confirmed via old Ritsu code |
| `PowerStackType` enum | `MegaCrit.Sts2.Core.Entities.Powers` | confirmed via old Ritsu code |
| `StrengthPower` etc. (power classes) | `MegaCrit.Sts2.Core.Models.Powers` | confirmed via STS2-resource dir |
| `ICombatant` | `MegaCrit.Sts2.Core.Entities` | confirmed via old Ritsu code |
| `PlayerManager` (static) | `MegaCrit.Sts2.Core.Entities.Players` | confirmed via old Ritsu code |
| `Player` (entity type) | `MegaCrit.Sts2.Core.Entities.Players` | BaseLib `PlayerExtensions.cs` |
| `CardModel` | `MegaCrit.Sts2.Core.Models` | BaseLib `CustomCardModel.cs` |
| `RelicModel` | `MegaCrit.Sts2.Core.Models` | BaseLib `CustomRelicModel.cs` |
| `PowerModel` | `MegaCrit.Sts2.Core.Models` | BaseLib `CustomPowerModel.cs` |
| `CardType` | `MegaCrit.Sts2.Core.Entities.Cards` | BaseLib `ConstructedCardModel.cs` |
| `CardRarity` | `MegaCrit.Sts2.Core.Entities.Cards` | BaseLib `ConstructedCardModel.cs` |
| `TargetType` | `MegaCrit.Sts2.Core.Entities.Cards` | BaseLib `ConstructedCardModel.cs` |
| `CardTag` | `MegaCrit.Sts2.Core.Entities.Cards` | BaseLib `ConstructedCardModel.cs` |
| `CardPlay` | `MegaCrit.Sts2.Core.Entities.Cards` | STS2-resource dir |
| `CardKeyword` | `MegaCrit.Sts2.Core.Entities.Cards` | BaseLib |
| `RelicRarity` | `MegaCrit.Sts2.Core.Entities.Relics` | STS2-resource dir |
| `PlayerChoiceContext` | `MegaCrit.Sts2.Core.GameActions.Multiplayer` | Fighter mod |
| `Faction` | `MegaCrit.Sts2.Core.Entities` | STS2-resource dir |
| `IntentType` | `MegaCrit.Sts2.Core.Entities` | STS2-resource dir |
| `PoolAttribute` | `BaseLib.Utils` | `/app/BaseLib-StS2/Utils/PoolAttribute.cs:6` |
| `ConstructedCardModel` | `BaseLib.Abstracts` | BaseLib |
| `CustomCardModel` | `BaseLib.Abstracts` | BaseLib |
| `CustomRelicModel` | `BaseLib.Abstracts` | BaseLib |
| `CustomPowerModel` | `BaseLib.Abstracts` | BaseLib |
| `DynamicVars` | `BaseLib.Extensions` | BaseLib |
| `CommonActions` | `BaseLib.Utils` | BaseLib |
| `ScriptManagerBridge` | `Godot.Bridge` | QuickReload `Entry.cs` |
| `SimpleLoc` | `BaseLib.Patches.Localization` | BaseLib |
| `PowerCmd` | `MegaCrit.Sts2.Core.Commands` | BaseLib |
| `DamageCmd` (static) | `MegaCrit.Sts2.Core.Commands` | BaseLib `CommonActions.cs` |
| `CardPileCmd` | `MegaCrit.Sts2.Core.Commands` | inferred |
| `PlaceholderCharacterModel` | `BaseLib.Abstracts` | inferred |

## Base Class Hierarchy

```
CardModel                      (MegaCrit.Sts2.Core.Models, game base)
  └─ CustomCardModel           (BaseLib.Abstracts, adds ICustomModel + ILocalizationProvider + auto-add)
      └─ ConstructedCardModel  (BaseLib.Abstracts, fluent builder: WithDamage/WithBlock/WithPower/WithTags)

RelicModel                     (MegaCrit.Sts2.Core.Models, game base)
  └─ CustomRelicModel          (BaseLib.Abstracts, adds ICustomModel + ILocalizationProvider + auto-add)

PowerModel                     (MegaCrit.Sts2.Core.Models, game base)
  └─ CustomPowerModel          (BaseLib.Abstracts, adds ILocalizationProvider)
```

## Card Builder API (ConstructedCardModel)

```csharp
public sealed class MyCard : FighterCardModel
{
    public MyCard() : base(cost, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithDamage(baseVal, upgradeVal);          // → DamageVar
        WithBlock(baseVal, upgradeVal);            // → BlockVar
        WithPower<SomePower>(baseVal, upgradeVal); // → PowerVar<T> + tooltip
        WithCards(baseVal, upgradeVal);            // → CardsVar (draw)
        WithEnergy(baseVal, upgradeVal);           // → EnergyVar + energy tip
        WithHeal(baseVal, upgradeVal);             // → HealVar
        WithTags(CardTag.Strike);
        WithKeywords(keyword);
        WithKeyword(keyword, UpgradeType.Remove);  // removed on upgrade
        WithCostUpgradeBy(-1);                     // reduce cost on upgrade
        WithVars(new CustomVar("name", baseVal));  // manual DynamicVar
        WithTip(tooltipSource);                    // custom hover tip
    }
}
```

## Power/Relic Inline Localization

Both `CustomPowerModel` and `CustomRelicModel` implement `ILocalizationProvider` with inline fallback:

```csharp
public override List<(string, string)>? Localization => [
    ("title", "显示名称"),
    ("description", "描述文字。*粗体* → [gold]粗体[/gold]"),
    ("smartDescription", "简短版描述"),
    ("flavor", "风味文字（仅遗物）")
];
```

JSON file keys take priority over inline. Key format: `PREFIX-CLASSNAME.field` (e.g., `FIGHTER-DEFEND_F.title`).

## Common Card Patterns

### OnPlay
```csharp
protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
{
    // Attack
    await CommonActions.CardAttack(this, cardPlay)
        .WithHitFx("vfx/vfx_bloody_impact")
        .Execute(choiceContext);

    // Block
    await CommonActions.CardBlock(this, cardPlay);
    // (block amount auto-reads from BlockVar)

    // Apply power
    await PowerCmd.Apply<SomePower>(choiceContext, target, amount, applier, cardSource);
    // Remove power
    await PowerCmd.Remove<SomePower>(choiceContext, target, amount, applier, cardSource);
}
```

### CanPlay (conditional playability)
```csharp
// NOTE: signature may differ from what CardModel actually exposes.
// Old Ritsu code used: public override bool CanPlay(PlayerChoiceContext context)
// BaseLib's ConstructedCardModel does NOT define this — it's on CardModel (game base).
// Verify against actual sts2.dll on Windows.
```

### GetCurrentEnergyCost (dynamic cost)
```csharp
// NOTE: signature may differ from what CardModel actually exposes.
// Old Ritsu code: public override int GetCurrentEnergyCost(PlayerChoiceContext context)
// BaseLib's ConstructedCardModel does NOT define this.
```

## Common Relic Patterns

```csharp
public sealed class MyRelic : Abstract.FighterRelicModel  // extends CustomRelicModel
{
    [Pool(typeof(FighterRelicPool))]
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool StarterExclusive => true;  // verify override exists on base

    // Combat start
    public override async Task OnCombatStart(PlayerChoiceContext choiceContext, Player player) { }
    // Turn start
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) { }

    // Static helpers (called from patches / cards)
    public static async Task DoSomething(PlayerChoiceContext choiceContext, int value) { }
}
```

## Common Power Patterns

```csharp
public sealed class MyPower : Abstract.FighterPowerModel  // extends CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;        // Buff / Debuff
    public override PowerStackType StackType => PowerStackType.Counter;  // Counter / Intensity

    // Turn-based power effects
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player) { }
}
```

## Harmony Patching

```csharp
[HarmonyPatch]     // can go on class (all methods target the same type)
public static class MyPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(TargetType), "MethodName")]  // on method
    static void Prefix(TargetType __instance) { }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TargetType), "MethodName")]
    static void Postfix() { }
}
```

**Static type limitations**: `DamageCmd` is a static class. You cannot use `DamageCmd __instance` as a Harmony parameter. If you need to patch a static method like `CalculateFinalDamage`, use `ref` parameters on the return value or use `Traverse` on the actual instance-based attack command class.

## Key APIs Quick Reference

| Task | API |
|------|-----|
| Register Godot scripts | `ScriptManagerBridge.LookupScriptsInAssembly(assembly)` from `Godot.Bridge` |
| Enable SimpleLoc | `SimpleLoc.EnableSimpleLoc(modId)` from `BaseLib.Patches.Localization` |
| Character registration | Extend `PlaceholderCharacterModel`, call `new Character()` in init |
| Apply power | `PowerCmd.Apply<T>(context, target, amount, applier, cardSource)` |
| Remove power | `PowerCmd.Remove<T>(context, target, amount, applier, cardSource)` |
| Check power | `player.GetPower<T>()` — extension from BaseLib |
| Play attack | `CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/...").Execute(ctx)` |
| Play block | `CommonActions.CardBlock(this, cardPlay)` |
| Get player | `PlayerManager.Player` static property |
| DynamicVar value | `DynamicVars.Power<T>().BaseValue` from `BaseLib.Extensions` |
| Pool registration | `[Pool(typeof(YourPool))]` on card/relic class, attribute from `BaseLib.Utils` |

## Fighter Mod Conventions

| Layer | Pattern | Example |
|-------|---------|---------|
| Namespace | `Fighter.Code.X` | `Fighter.Code.Cards.Basic` |
| BaseLib prefix | `FIGHTER-CLASSNAME` (dash) | `FIGHTER-DEFEND_F` |
| Card base | `FighterCardModel` extends `ConstructedCardModel` | — |
| Power base | `FighterPowerModel` extends `CustomPowerModel` | — |
| Relic base | `FighterRelicModel` extends `CustomRelicModel` | — |
| Localization dir | `Fighter/localization/zhs/` | cards.json, relics.json, powers.json |

## Common Errors

| Code | Cause | Fix |
|------|-------|-----|
| CS0246 (PowerType/PowerStackType) | `using Models.Powers` instead of `Entities.Powers` | Classes are in `Models.Powers`, ENUMS are in `Entities.Powers`. Need BOTH usings |
| CS0246 (ICombatant) | Missing `using MegaCrit.Sts2.Core.Entities` | Add the using |
| CS0246 (PoolAttribute/Pool) | Missing `using BaseLib.Utils` | Add the using |
| CS0246 (ScriptManagerBridge) | Missing `using Godot.Bridge` | Add the using |
| CS0721 (static type as parameter) | Used `DamageCmd __instance` in Harmony prefix | DamageCmd is static. Use `ref` on result params |
| CS0436 (Main type conflict) | Godot source gen vs sts2.dll | Add `CS0436` to `<NoWarn>` in csproj |
| MSB4126 (invalid solution config) | Corrupted .sln encoding | Build from `.csproj`, not `.sln` |
| CS0118 (namespace as type) | Namespace `Fighter` collides with class `Fighter` | Use `global::Fighter.Code.Character.Fighter()` |
| Cards show class names | JSON key format mismatch | Use `PREFIX-CLASSNAME` (dash), NOT `PREFIX_CATEGORY_CLASSNAME` |
| localization not loading | Files not copied to mods folder | Add copy step in csproj post-build target |
| CS0115 (no method to override) | Method doesn't exist on game base class | Verify method name + signature against actual sts2.dll. Some methods (CanPlay, GetCurrentEnergyCost) may not be virtual on CardModel. |

## Reference Locations

| Resource | Path |
|----------|------|
| Decompiled STS2 game (dir structure only) | `/app/STS2-resource/src/` |
| BaseLib source | `/app/BaseLib-StS2/` |
| Working reference mod (QuickReload) | `/app/STS2-QuickReload/` |
| Old RitsuLib code (excluded from build) | `/app/STS2-Fighter/src_old_ritsu/` |
| Fighter mod source | `/app/STS2-Fighter/Code/` |
| NuGet cache | `~/.nuget/packages/alchyr.sts2.baselib/` |
