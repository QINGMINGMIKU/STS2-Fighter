# Fighter Mod — 工作日志 (2026-05-01)

## 构建状态

`dotnet build -c Release` 通过，0 错误 0 警告。
`dotnet build -t:ExportPck` 导出 PCK（Godot 4.5.1）。

## 项目规模

- **28 张卡** (Basic 6 / Common 13 / Uncommon 7 / Rare 4)
- **9 个 Power** (Combo, Cancel, FrameAdvantage, FightingSpirit, SuperGauge, StrikeThrowMixup, TCPower, ConfirmPower, WhiffPunish, YiJianlianPower)
- **3 个遗物** (FighterHeadband, SpiritCharm, SuperArtTalisman)
- **6 个关键词** (投技, 起手, 连击, 取消, 必杀, 超必杀)

## 初始卡组 (10张)

4×防御, 1×打击, 2×轻击, 1×重击, 1×指令投, 1×绿冲

## 机制系统

| 系统 | 实现 |
|------|------|
| 帧数净值 | FrameAdvantage (AllowNegative=true), 可透支 |
| 打康 A1/B/A2 | FighterHeadband.ModifyDamageMultiplicative + AfterDamageReceived |
| 连击 | 起手→Combo Power(上限1)→连击消耗→0费+抽牌(TC) |
| 斗气 | FightingSpirit(上限6), burnout 2回合, SpiritHelper |
| 取消 | Cancel N→施加Cancel Power→必杀/超必杀消费-1费 |
| 必杀 | 消耗2斗气; burnout时改为能量+2×帧数 |
| 超必杀 N | 消耗N段超杀槽, 回复3斗气, 消耗所有帧 |
| 超杀槽 | SuperArtTalisman遗物, 每3攻击+1段(上限3), 跨战斗保留, 计数器显示 |
| 冻原 | FighterHeadband跟踪被缓冲抵消伤害, 冻原风暴返还 |

## 工作流程

1. `dotnet build -c Release` → DLL
2. `dotnet build -t:ExportPck` → PCK (Godot 4.5.1)
3. PCK + DLL + Fighter.json → `mods/Fighter/`

## 未完成

- 易建联抽牌逻辑 (API不兼容, 仅80%减伤生效)
- 大量 SF6 卡牌待设计 (见 SF6-全人物必杀技卡牌设计.md)
- 卡牌效果描述需最终校准 (见 cards.md)
- EnergyColorName 暂用 "red", 需自定义图标
