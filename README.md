# Fighter — Slay the Spire 2

A Street Fighter inspired character mod for Slay the Spire 2, built with RitsuLib.

**Frame Advantage system** — manage positive/negative frames to control the flow of combat. Land counter-hits, execute combos, and unleash super arts.

## Features

- **46 cards** across Basic, Common, Uncommon, and Rare rarities
- **Frame Advantage** — spendable positive frames, punishable negative frames
- **Counter-hit system** — bonus damage when attacking enemies with Attack intent
- **Combo system** — open with Starters, consume Combo stacks for bonus effects
- **Fighting Spirit** — stackable resource granting STR/DEX, consumed by special moves
- **Super Gauge** — builds with attacks, spent on devastating super arts
- **Tipsy mechanic** — drunken fist chain with escalating effects based on stack count
- **10 powers**, **3 relics**, custom keywords

## Dependencies

- [STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) ≥ 0.2.15

## Project Structure

```
├── Fighter.json          # Mod manifest
├── Fighter.csproj        # .NET 9.0 project
├── project.godot         # Godot 4.5 project
├── export_presets.cfg    # PCK export preset
├── Scripts/
│   ├── Entry.cs          # Mod entry point
│   ├── Cards/            # Card implementations
│   ├── Powers/           # Power implementations
│   ├── Relics/           # Relic implementations
│   ├── Character/        # Character, card/relic/potion pools
│   ├── Mechanics/        # Helper classes (counter-hit, spirit, etc.)
│   └── Keywords/         # Custom keyword registration
└── Fighter/
    ├── images/           # Card portraits, power/relic icons
    ├── audio/            # Sound effects
    ├── fonts/            # Custom fonts
    └── localization/     # Multi-language card/power/relic text
```

## Build

1. Set `Sts2Dir` and `GodotExe` in `Fighter.csproj` to match your system.
2. Run `dotnet build -c Release` to compile and copy to the mods folder.
3. To export PCK: run the `ExportPck` MSBuild target, or manually:
   ```
   godot --headless --export-pack "BasicExport" Fighter.pck
   ```

## Development Notes

This mod uses RitsuLib's auto-registration system (`[RegisterCard]`, `[RegisterPower]`, `[RegisterRelic]`). All cards extend `ModCardTemplate`, powers extend `ModPowerTemplate`. Localization keys follow the pattern `FIGHTER_CARD_XXX.title` / `FIGHTER_POWER_XXX.title`.

Card art placeholder images are expected under `Fighter/images/card_portraits/` and power icons under `Fighter/images/powers/`.

## Credits

- Framework: [STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) by BAKAOLC
