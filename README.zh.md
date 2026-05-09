# 格斗家 — 杀戮尖塔 2

基于 RitsuLib 开发的格斗家角色模组，灵感来源于街头霸王系列。

**帧数净值系统** — 通过正负帧管理掌控战斗节奏。打康、确反、连击、超必杀，还原格斗游戏的立回体验。

## 特色

- **46 张卡牌** — 覆盖基础、普通、罕见、稀有四个稀有度
- **帧数净值** — 正帧可消耗用于追加效果，负帧陷入破绽被敌人打康
- **打康系统** — 攻击进攻意图的敌人获得额外伤害和帧数
- **连击系统** — 起手牌施加连击状态，连击牌消耗连击触发追加效果
- **斗气** — 上限 6 层，持有期间 +1 力量 +1 敏捷，每回合恢复 1 层
- **超杀槽** — 每打出 3 张攻击牌积攒 1 段（上限 3 段），超必杀技消耗超杀槽发动
- **醉意机制** — 醉拳体系，根据醉意层数触发递增的卡牌效果
- **10 种能力**、**3 个遗物**，自定义关键词

## 依赖

- [STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) ≥ 0.2.15

## 项目结构

```
├── Fighter.json          # 模组清单
├── Fighter.csproj        # .NET 9.0 项目
├── project.godot         # Godot 4.5 项目
├── export_presets.cfg    # PCK 导出预设
├── Scripts/
│   ├── Entry.cs          # 模组入口
│   ├── Cards/            # 卡牌
│   ├── Powers/           # 能力
│   ├── Relics/           # 遗物
│   ├── Character/        # 角色、卡池、遗物池、药水池
│   ├── Mechanics/        # 机制辅助类
│   └── Keywords/         # 自定义关键词注册
└── Fighter/
    ├── images/           # 卡牌立绘、能力/遗物图标
    ├── audio/            # 音效
    ├── fonts/            # 字体
    └── localization/     # 多语言本地化
```

## 构建

1. 在 `Fighter.csproj` 中设置 `Sts2Dir` 和 `GodotExe` 路径。
2. 运行 `dotnet build -c Release` 编译并复制到 mods 目录。
3. 导出 PCK：运行 `ExportPck` MSBuild 目标，或手动执行：
   ```
   godot --headless --export-pack "BasicExport" Fighter.pck
   ```

## 开发说明

本模组使用 RitsuLib 的自动注册系统（`[RegisterCard]`、`[RegisterPower]`、`[RegisterRelic]`）。所有卡牌继承 `ModCardTemplate`，能力继承 `ModPowerTemplate`。本地化 key 遵循 `FIGHTER_CARD_XXX.title` / `FIGHTER_POWER_XXX.title` 格式。

卡牌立绘占位图放在 `Fighter/images/card_portraits/`，能力图标放在 `Fighter/images/powers/`。

## 致谢

- 框架：[STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) by BAKAOLC
