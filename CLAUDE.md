# CLAUDE.md — STS2-Fighter

This file provides guidance for working on the Fighter character mod.

## Build

```cmd
dotnet build Fighter.csproj -c Release
```

Always build from `.csproj`, not `.sln`.

## Architecture

```
Code/
├── FighterMainFile.cs          ← Entry: [ModInitializer], Harmony init, SimpleLoc, new Fighter()
├── Character/
│   ├── Fighter.cs              ← PlaceholderCharacterModel (Ironclad visuals)
│   ├── FighterCardPool.cs
│   ├── FighterRelicPool.cs
│   └── FighterPotionPool.cs
├── Abstract/
│   ├── FighterCardModel.cs     ← ConstructedCardModel (portrait paths)
│   ├── FighterPowerModel.cs    ← CustomPowerModel (icon paths)
│   └── FighterRelicModel.cs    ← CustomRelicModel (icon paths)
├── Cards/Basic/                ← Starting deck cards (5 cards)
├── Powers/                     ← FrameAdvantage, FightingSpirit, ThrowStun
├── Relics/                     ← FighterHeadband (counter-hit), SpiritCharm (spirit system)
├── Mechanics/                  ← CounterHitState, IFighterTagged
├── Nodes/                      ← Godot nodes (creature visuals, energy counter, etc.)
└── Patches/                    ← ColorfulPhilosophers, audio, energy counter, progress save
```

Naming: namespace `Fighter.Code.*` → BaseLib prefix `FIGHTER-` → model entry `FIGHTER-CLASSNAME`.

## Design: Starting Deck (10 cards)

| Count | Class | Cost | Effect |
|-------|-------|------|--------|
| 4 | Defend_F | 1 | 5 block (+3), Defend tag |
| 2 | Strike_L | 0 | 4 dmg (+3), triggers combo starter flag |
| 2 | Strike_H | 2 | 8 dmg (+3), combo: 0-cost + consume 3 frames |
| 1 | CommandGrab | 2 | 13 dmg (+4), consume 6 frames, both stunned |
| 1 | DriveRush | 0 | +4 frames (+2), consume 2 Fighting Spirit |

## Design: Mechanics

- **FrameAdvantage** — net frame resource (positive = spendable, negative = vulnerable)
- **Counter Hit (A1)** — player attacks enemy with Attack intent → +20% dmg, +2 frames
- **Counter Hit (A2)** — enemy attacks player at negative frames → +20% dmg to enemy
- **Punish Counter (B)** — player fully blocks enemy Attack → next attack +20% dmg, +4 frames
- **Combo (TC)** — Strike_L marks combo starter → Strike_H costs 0 and consumes 3 frames
- **Fighting Spirit** — 6 stacks at combat start, +1 STR/DEX while active, per-turn +1
- **Burnout** — Spirit hits 0 → 2 Weak + 2 Vulnerable, refills to 6
- **Throw Stun** — CommandGrab stuns both sides for 1 turn (cannot attack)

## Key APIs

| Task | API |
|------|-----|
| Register Godot scripts | `ScriptManagerBridge.LookupScriptsInAssembly(assembly)` from `Godot.Bridge` |
| Enable SimpleLoc | `SimpleLoc.EnableSimpleLoc(modId)` from `BaseLib.Patches.Localization` |
| Character registration | Extend `PlaceholderCharacterModel`, call `new Character()` in init |
| Card base | Extend `ConstructedCardModel`, use `WithDamage()`, `WithBlock()`, `WithPower<T>()`, `WithTags()` |
| Apply power | `PowerCmd.Apply<T>(context, target, amount, applier, cardSource)` |
| Remove power | `PowerCmd.Remove<T>(context, target, amount, applier, cardSource)` |
| Check power | `player.GetPower<T>()` — game extension method |
| Play attack | `CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/...").Execute(ctx)` |
| Play block | `CommonActions.CardBlock(this, cardPlay)` |
| Draw cards | `CardPileCmd.Draw(choiceContext, count, owner)` |
| DynamicVar access | `DynamicVars.Power<T>().BaseValue` from `BaseLib.Extensions` |

## Localization

JSON keys use BaseLib format: `FIGHTER-CLASSNAME.key` (NOT the old `FIGHTER_CARD_CLASSNAME` format).
JSON files are in `Fighter/localization/zhs/`. Post-build copies them to mods folder.
Powers and relics also have inline `ILocalizationProvider.Localization` as fallback.

## Reference

- Decompiled STS2 game: `/app/STS2-resource/src/`
- BaseLib source: `/app/BaseLib-StS2/`
- Old RitsuLib code (excluded from build): `src_old_ritsu/`
- Working reference mod: `/app/STS2-QuickReload/`
