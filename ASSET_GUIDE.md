# Fighter Mod 美术资源制作指南

## 项目信息

- **Mod ID**: `Fighter`（格斗家，街霸6同人角色）
- **能量颜色**: `fighterRed`（橙红）
- **角色配色**: `#D91F14`（朱红），MapDrawing `(0.85, 0.12, 0.08)`
- **Godot 项目**: `/app/STS2-Fighter/project.godot`
- **资源根目录**: `res://Fighter/`（即 `/app/STS2-Fighter/Fighter/`）

---

## 一、制作优先级

| 优先级 | 类别 | 数量 | 影响 |
|--------|------|------|------|
| **P0 必做** | 能力图标 | 12能力×2尺寸 | 战斗中所有 buff/debuff 显示 |
| **P0 必做** | 遗物图标 | 3遗物×3种图 | 遗物栏和获得界面 |
| **P0 必做** | 卡牌立绘 | 35张 | 卡牌美术 |
| **P0 必做** | 能量计数器层 | 5层 | 战斗中能量球显示 |
| **P1 重要** | 能量UI图标 | 2张 | 卡牌文字中的能量小图标 |
| **P1 重要** | 角色选择资源 | ~5张 | 选角色界面 |
| **P2 进阶** | 角色Spine动画 | 整套 | 战斗中角色动画 |
| **P3 打磨** | VFX粒子贴图 | ~6张 | 攻击特效/拖尾 |

---

## 二、能力图标（P0）

**目录**: `Fighter/images/powers/`

每个能力需要两个 PNG 文件。参考原版图标风格：圆形底 + 简洁符号。

| 文件名 | 尺寸 | 说明 |
|--------|------|------|
| `frame_advantage.png` | 64×64 | 帧数净值，正=优势/负=破绽 |
| `frame_advantage_big.png` | 128×128 | 同上大图 |
| `fighting_spirit.png` | 64×64 | 格斗精神/斗志 |
| `fighting_spirit_big.png` | 128×128 | 同上大图 |
| `super_gauge.png` | 64×64 | 超必杀槽 |
| `super_gauge_big.png` | 128×128 | 同上大图 |
| `combo.png` | 64×64 | 连击标记（轻击起手后的连携） |
| `combo_big.png` | 128×128 | 同上大图 |
| `cancel.png` | 64×64 | 取消/绿冲取消 |
| `cancel_big.png` | 128×128 | 同上大图 |
| `tipsy.png` | 64×64 | 微醺/醉酒debuff |
| `tipsy_big.png` | 128×128 | 同上大图 |
| `whiff_punish.png` | 64×64 | 挥空惩罚 |
| `whiff_punish_big.png` | 128×128 | 同上大图 |
| `confirm.png` | 64×64 | 确反确认 |
| `confirm_big.png` | 128×128 | 同上大图 |
| `tc.png` | 64×64 | TC（Target Combo）标记 |
| `tc_big.png` | 128×128 | 同上大图 |
| `devils_song.png` | 64×64 | 恶魔之歌 |
| `devils_song_big.png` | 128×128 | 同上大图 |
| `strike_throw_mixup.png` | 64×64 | 打投二择折扣 |
| `strike_throw_mixup_big.png` | 128×128 | 同上大图 |
| `yi_jianlian.png` | 64×64 | 一键连 |
| `yi_jianlian_big.png` | 128×128 | 同上大图 |

> 共 24 个文件（12 能力 × 2 尺寸）

---

## 三、遗物图标（P0）

**目录**: `Fighter/images/relics/`

每个遗物需要 3 个 PNG：图标、轮廓、大图。轮廓图用于未获得时的灰色预览。

| 文件名 | 尺寸 | 说明 |
|--------|------|------|
| `fighter_headband.png` | 128×128 | 格斗家头带（起始遗物） |
| `fighter_headband_outline.png` | 128×128 | 轮廓（单色，透明背景） |
| `fighter_headband_big.png` | 256×256 | 大图（获得弹窗用） |
| `spirit_charm.png` | 128×128 | 精神护符（起始遗物） |
| `spirit_charm_outline.png` | 128×128 | 轮廓 |
| `spirit_charm_big.png` | 256×256 | 大图 |
| `super_art_talisman.png` | 128×128 | 超必杀护符 |
| `super_art_talisman_outline.png` | 128×128 | 轮廓 |
| `super_art_talisman_big.png` | 256×256 | 大图 |

> 共 9 个文件（3 遗物 × 3 种图）

---

## 四、卡牌立绘（P0）

**目录**: `Fighter/images/card_portraits/`

卡牌立绘尺寸参考原版：**250×190 px**，放在卡牌画框内。

### 起始卡组（Basic）

| 文件名 | 卡牌 | 费用 | 效果 |
|--------|------|------|------|
| `strike_l.png` | 轻击 | 0 | 4伤，挂[连击]标记 |
| `strike_h.png` | 重击 | 2 | 8伤，连击时0费+耗3帧 |
| `defend_f.png` | 防御 | 1 | 5格挡 |
| `command_grab.png` | 投技 | 2 | 13伤不可格挡，耗6帧 |
| `drive_rush.png` | 绿冲 | 0 | +4帧，耗2精神 |

### 普通（Common）

| 文件名 | 卡牌 |
|--------|------|
| `hadoken.png` | 波动拳 |
| `shoryuken.png` | 升龙拳 |
| `tatsumaki.png` | 龙卷旋风腿 |
| `cannon_spike.png` | 加农 spike |
| `cannon_strike.png` | 加农 strike |
| `dragonlash_kick.png` | 龙蹬踢 |
| `hell_spike.png` | 地狱刺 |
| `luminous_dive_kick.png` | 光耀俯冲踢 |
| `safe_jump.png` | 安全跳 |
| `spiral_arrow.png` | 螺旋箭 |
| `strike_throw_mixup.png` | 打投二择 |
| `taunt.png` | 挑拨 |
| `the_devil_inside.png` | 心魔 |
| `whiff_punish.png` | 挥空惩罚 |

### 罕见（Uncommon）

| 文件名 | 卡牌 |
|--------|------|
| `bakkai.png` | 莫邪剑·拔开 |
| `confirm.png` | 确认 |
| `double_lariat.png` | 双截拳 |
| `jinrai_kick.png` | 迅雷踢 |
| `quick_dash.png` | 疾冲 |
| `quick_spin_knuckle.png` | 快旋拳 |
| `tc.png` | TC |
| `tenshin.png` | 天心 |
| `the_devils_song.png` | 恶魔之歌 |
| `yi_jianlian.png` | 一键连 |

### 稀有（Rare）

| 文件名 | 卡牌 |
|--------|------|
| `ashura_senku.png` | 阿修罗闪空 |
| `getsuga_saiho.png` | 月牙碎崩 |
| `grand_storm.png` | 大岚 |
| `tundra_storm.png` | 冻土岚 |

> 共 35 张卡牌立绘

---

## 五、能量计数器层（P0）

**目录**: `Fighter/images/ui/combat/energy_counters/`

能量球由 5 层叠加旋转构成。

| 文件名 | 尺寸 | 行为 |
|--------|------|------|
| `layer1.png` | 128×128 | 底层，**不动** |
| `layer2.png` | 128×128 | 旋转层，慢速 |
| `layer3.png` | 128×128 | 旋转层，中速（比layer2快） |
| `layer4.png` | 128×128 | 顶层，不动 |
| `layer5.png` | 128×128 | 顶层，不动 |

**制作流程**：
1. 在 Photoshop 中画好五层合并的 `EnergyCounter.psd`（已有）
2. 导出为 5 个独立的 128×128 PNG（已导出 ✅）
3. 如需修改：编辑 PSD → 重新导出各层 PNG

**场景文件**: `Fighter/scenes/combat/energy_counters/fighter_energy_counter.tscn` ✅

**待完善**：VFX 粒子（EnergyVfxBack / EnergyVfxFront 节点）
- 需要在 Godot 编辑器中打开场景，给这两个节点挂 `NParticlesContainer` 脚本
- 添加 `GpuParticles2D` 子节点，使用以下贴图：
  - `energy_orb_burst.png` — 能量爆发
  - `energy_orb_shine.png` — 能量闪光
  - `common_glow.png` — 通用光晕

---

## 六、能量 UI 图标（P1）

**目录**: `Fighter/images/ui/`

| 文件名 | 尺寸 | 用途 | C# 属性 |
|--------|------|------|---------|
| `energy_fighter_big.png` | ~64×64 | 大号能量图标 | `BigEnergyIconPath` |
| `energy_fighter.png` | ~24×24 | 卡牌文字内联能量符号 | `TextEnergyIconPath` |

此外，修改了 `EnergyColorName` 为 `fighterRed`。卡牌文字中的能量图标会查找：
```
atlases/ui_atlas.sprites/card/energy_fighterred.tres
```
这个 `.tres` 需要在 Godot 里创建：指向 UI atlas 中 Fighter 能量图标的位置。

> 如果暂时不想动 atlas，可以先把 `EnergyColorName` 改回 `"red"`（共用铁甲战士的红色能量图标）。

---

## 七、角色选择 / 地图资源（P1）

**目录**: `Fighter/images/watcher/` → 需要全部替换为 Fighter 主题

| 当前占位文件 | 用途 | 建议尺寸 |
|-------------|------|---------|
| `bg_watcher.png` | 角色选择背景 | 1920×1080 |
| `bg_back.png` | 背景后层 | 1920×1080 |
| `char_select_watcher.png` | 已解锁角色选择头像 | ~512×512 |
| `char_select_watcher_locked.png` | 未解锁角色选择头像 | ~512×512 |
| `character_icon_watcher.png` | 小图标（跑团界面用） | 128×128 |
| `character_icon_watcher_outline.png` | 小图标轮廓 | 128×128 |
| `map_marker_watcher.png` | 地图上的角色标记 | 64×64 |

**建议**：新建目录 `Fighter/images/fighter/`，放 Fighter 的角色图，然后更新 C# 代码中的 asset profile 路径引用。

---

## 八、角色 Spine 动画（P2）

角色在战斗中显示为 Spine 骨骼动画。目前用的是 Watcher 的骨架。如果要完全自制：

- Spine 图集: `Watcher.png` → 替换为 Fighter 的骨骼贴图
- 身体部件: `body.png`, `arm_left.png`, `arm_right.png`
- 特殊状态: `sitting.png`（休息）, `corpse.png`（死亡）
- 眼部动画: `eye/the_watcher_eye.png`
- 过渡动画: `transitions/watcher_transition.png`

如果想省事先用 Ironclad 的骨架（`FighterCharacter` 当前继承 `Ironclad` 视觉），就不需要 Spine 资源，但需要改代码里的 `VisualsPath`。

---

## 九、多人模式手势（P2）

**目录**: `Fighter/images/watcher/hands/`

| 文件 | 用途 |
|------|------|
| `m_arm.png` | 手臂 |
| `m_point.png` | 指 |
| `m_rock.png` | 石头 |
| `m_paper.png` | 布 |
| `m_scissors.png` | 剪刀 |
| `multiplayer_hand_watcher_*.png` (4张) | 多人联机手部图标 |

---

## 十、VFX 粒子贴图（P3）

**目录**: `Fighter/images/vfx/`

| 文件 | 用途 |
|------|------|
| `trail.png`, `trail2.png` | 卡牌拖尾 |
| `small_card_silhouette.png` | 小卡牌剪影 |
| `glow_spark.png` | 火花粒子 |
| `strike_line.png` | 打击线 |
| `big_blur.png` | 大模糊 |
| `brush_particle_2.png` | 笔刷粒子 |
| `screenflash.png` | 屏幕闪光 |
| `frost_streak.png` | 冰霜条纹 |
| `sparkle.png` | 闪烁 |

> 这些可以沿用原版，但想要风格统一的话建议替换。

---

## 十一、材质文件（已有，不需改动）

| 文件 | 用途 | 状态 |
|------|------|------|
| `Fighter/materials/ui/energy_orb_dark.tres` | 能量=0时的暗化效果 | ✅ |
| `Fighter/materials/vfx_glow.tres` | 光晕材质 | ✅ |
| `Fighter/materials/vfx_speck_glow_white.tres` | 白色粒子光斑 | ✅ |
| `Fighter/themes/kreon_bold_shared.tres` | 字体 | ✅ |
| `Fighter/themes/canvas_item_material_additive_shared.tres` | 加色混合 | ✅ |

---

## 十二、Godot 操作速查

```
导入项目：   双击 project.godot 用 Godot 4.5.1 Mono 打开
修改场景：   FileSystem → 找到文件 → 双击打开
导出 .pck：  顶部菜单 Project → Export → BasicExport → Export
命令行导出： Godot_v4.5.1 --headless --export-pack "BasicExport" "mods/Fighter/Fighter.pck"
重新导入：   右键 PNG → Reimport
```

每次导出 .pck 后复制到 STS2 的 mods 目录即可测试。

---

## 制作顺序建议

```
Week 1-2: P0 能力图标 + 遗物图标（先让 build 不报缺，进游戏能看到）
Week 3-4: P0 卡牌立绘（35张，量大，可以逐步替换）
Week 5:   P0 能量计数器 VFX + P1 能量UI图标
Week 6:   P1 角色选择界面资源
Week 7+:  P2 角色 Spine（这个需要 Spine 软件 + 骨骼绑定）
Week 8+:  P3 VFX打磨
```
