# CLAUDE.md — STS2-Fighter

This file provides guidance for working on the Fighter character mod.

## Build

```cmd
dotnet build Fighter.csproj -c Release
```

Always build from `.csproj`, not `.sln`. Post-build copies `.dll`, `.pdb`, and `Fighter.json` to the game's `mods/Fighter/`.

## Architecture (RitsuLib Framework)

All source in `Scripts/`. Uses `ModCardTemplate`, `ModPowerTemplate`, `ModRelicTemplate`, `ModCharacterTemplate` from `STS2RitsuLib.Scaffolding.*`. Auto-registration via `[RegisterCard]`, `[RegisterPower]`, `[RegisterRelic]`, `[RegisterCharacter]`.

**This codebase was migrated from BaseLib. The old pattern (`ConstructedCardModel`, `CustomPowerModel`, `FighterCardModel`) is gone — do not follow it.**

```
Scripts/
├── Entry.cs                     ← [ModInitializer], RitsuLibFramework pipeline
├── FrameHelper.cs               ← SecondaryResourceCmd wrapper for frame_advantage
├── FighterResources.cs          ← Secondary resource ID constants
├── Character/
│   ├── FighterCharacter.cs      ← ModCharacterTemplate<FighterCardPool, FighterRelicPool, FighterPotionPool>
│   ├── FighterCardPool.cs
│   ├── FighterRelicPool.cs
│   └── FighterPotionPool.cs     ← stub (no potions yet)
├── Cards/
│   ├── Basic/                   ← Strike_Fighter, Defend_F, Strike_L, Strike_H
│   ├── Common/                  ← 15 cards (CannonSpike, Shoryuken, Hadoken, etc.)
│   ├── Uncommon/                ← 11 cards (JinraiKick, Tenshin, QuickDash, etc.)
│   └── Rare/                    ← 4 cards (GrandStorm, GetsugaSaiho, AshuraSenku, TundraStorm)
├── Powers/                      ← 12 powers including FighterInnatePower (spirit system)
├── Relics/                      ← FighterHeadband (starter), SuperArtTalisman (starter)
├── Mechanics/                   ← CancelHelper, CounterHitState, SpiritHelper, TipsyHelper, TurnState
├── Nodes/                       ← Godot UI nodes (FighterFrameCounter, FighterSuperGauge, FightingSpiritGauge)
├── Patches/                     ← FighterCombatUiPatch (injects fighter gauges into combat UI)
└── Keywords/
    └── FighterKeywords.cs       ← 7 keywords: Throw, Starter, Combo, Cancel, Special, Super, Tipsy
```

## Secondary Resources

| Resource | Const | Default | Max | Persistence |
|----------|-------|---------|-----|-------------|
| 帧数净值 | `FighterResources.FrameAdvantage` | 0 | ±∞ | Combat |
| Super Gauge | `FighterResources.SuperGauge` | 0 | 3 | Run |
| 斗气 | `FighterResources.FightingSpirit` | 0 | 6 | Combat |

**Critical:** Registration in `Entry.cs` and all usage must use the same `FighterResources.*` constant.

## Starting Deck (10 cards)

| Count | Class | Cost | Effect |
|-------|-------|------|--------|
| 4 | Defend_F | 1 | 5 block (+3) |
| 2 | Strike_L | 0 | 4 dmg (+3), Starter tag, applies Combo |
| 2 | Strike_H | 2 | 8 dmg (+3), Combo: 0-cost, consume 3 frames |
| 1 | CommandGrab | 2 | 13 dmg (+4), unblockable, consume 6 frames, Throw tag |
| 1 | DriveRush | 0 | +4 frames (+2), consume 2 spirit (insufficient: -25% effect) |

## Mechanics

- **FrameAdvantage** — positive frames spendable on cancel/special; negative = enemy counter-hit
- **Counter Hit A1** — attack enemy with Attack intent → +20% dmg, +2 frames (FighterHeadband)
- **Counter Hit A2** — enemy attacks player at negative frames → +20% to enemy
- **Punish Counter B** — fully block enemy attack → next attack +20% dmg, +4 frames
- **Combo** — Starter cards grant Combo; Strike_H costs 0 when Combo active
- **Cancel** — Cancel N card consumes stacks; next Special/Super costs -1 energy
- **Fighting Spirit** — 6 stacks at combat start, +1 STR/DEX while active, per-turn +1; 0 = Burnout
- **Burnout** — 2 turns of 2 Weak + 2 Vulnerable, then refills to 6
- **Super Gauge** — built via SuperArtTalisman; CA: +10% damage at ≤25% HP

## Localization

JSON keys use format: `FIGHTER_CARD_CLASSNAME.title` / `FIGHTER_POWER_CLASSNAME.title`.

Files in `Fighter/localization/<lang>/`: `cards.json`, `powers.json`, `relics.json`, `characters.json`, `card_keywords.json`, `potions.json`.

7 languages: zhs (simplified), eng, deu, ita, jpn, kor, rus. zhs and eng are the authoritative files.

## Reference

- Decompiled game source: `../STS2-resource/src/Core/`
- RitsuLib source: `../STS2-RitsuLib/src/`
