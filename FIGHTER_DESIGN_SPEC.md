# 格斗家 (Fighter) 卡牌设计规格书

## 给 AI 生成卡牌用的完整设计参考

---

## 1. 角色身份

- **Mod ID**: `Fighter`
- **本地化前缀**: `FIGHTER-`（BaseLib 格式，破折号分隔）
- **定位**: 帧数格斗家。Street Fighter 6 同人角色，核心机制围绕"帧数优势"和"打康/确反康"系统。

---

## 2. 资源系统（3 种核心资源）

### 2.1 帧数净值 (FrameAdvantage)

- **类型**: Buff，堆叠方式 Counter，允许负值 (`AllowNegative = true`)
- **含义**: 正帧 = 玩家的有利帧（可消耗资源）；负帧 = 破绽状态（被敌人打康）
- **来源**: 绿冲(+4)、轻击攻击进攻敌人(+2)、确反康成功(+4)
- **消耗**: 重击连击(-3)、投技(-6)、以及其他消耗帧数的卡
- **无上下限**（理论上可以无限正帧或无限负帧）

**设计规则**:
- 正帧是"半货币"——可花也可攒。一般 2 帧 ≈ 1 费的价值
- 负帧是"自残状态"——负帧时被敌人攻击会受到 +20% 伤害（A2 打康）
- 没有帧数门槛的卡总是可打出的（即使负帧），但有帧数条件的卡需要检查

### 2.2 斗气 (FightingSpirit)

- **类型**: Buff，堆叠方式 Counter，初始 6 层，上限 6
- **效果**: 有斗气时自动获得 +1 力量 +1 敏捷（由 `EnsureStatBonus` 维持）
- **恢复**: 每回合开始自动 +1 层（不超过 6）
- **消耗**: 绿冲消耗 2 层斗气；其他卡可设计消耗斗气
- **Burnout 机制**: 斗气降至 0 时触发 → 清除力量和敏捷加成 → 获得 2 层虚弱 + 2 层脆弱 → 2 回合后斗气回满 6 层

**设计规则**:
- 斗气是"续航-爆发"平衡器。消耗斗气 = 失去本回合力量/敏捷加成 + 更接近 burnout
- Burnout 惩罚严重（虚弱 = -25% 伤害 + 脆弱 = +50% 受伤），但只有 2 回合
- 1 层斗气 ≈ 0.5 费的价值（因为同时意味着 +1 力 +1 敏的损失风险）

### 2.3 超杀槽 (SuperGauge)

- **类型**: Buff，堆叠方式 Counter，上限 3 段
- **来源**: 每打出 3 张攻击牌获得 1 段（由 SuperArtTalisman 遗物追踪）
- **消耗**: 超杀技消耗（目前未实现）——设计空间预留

**设计规则**（预留）:
- 超杀技应是高费高收益的攻击牌，消耗 1-3 段超杀槽
- 1 段超杀 ≈ 1 费的额外价值
- 超杀技建议稀有度 Rare 或更高

---

## 3. 关键字

| 关键字 | 效果 | 说明 |
|--------|------|------|
| **Throw (投)** | 未实现额外效果 | 标记投技，目前仅用于 AI/遗物识别 |
| **Heavy (重)** | 未实现额外效果 | 标记重击，目前仅用于 AI/遗物识别 |
| **连击 (Combo)** | 最多 1 层 Buff | Strike_L 施加，Strike_H 可消耗实现 0 费 |

---

## 4. 核心战斗机制

### 4.1 打康 (Counter Hit) — 三条件自动触发

修改器实现在 `CounterHitBuff.ModifyDamageMultiplicative()` 中，**无需在卡牌代码中手动处理**。打康效果自动判定：

| 类型 | 条件 | 效果 | 触发者 |
|------|------|------|--------|
| **A1 打康** | 玩家正帧 + 攻击具有 Attack 意图的敌人 | 玩家伤害 ×1.20 | 玩家攻击时 |
| **A2 被打康** | 玩家负帧 + 被敌人攻击 | 敌人伤害 ×1.20 | 玩家受伤时 |
| **B 确反康** | 玩家完全格挡敌人 Attack → 下一次攻击 | 玩家伤害 ×1.20 + 获得 4 帧 | 玩家攻击时 |
| **B 确反失败惩罚** | 确反康攻击未能破防 | 敌人伤害 ×1.20（本回合剩余） | 玩家受伤时 |

**设计规则**:
- 卡牌设计不直接操作伤害倍率，只需要关注帧数状态
- 鼓励"攒正帧+攻击进攻敌人"的节奏——这是 A1 触发的核心玩法
- 格挡技能的价值被确反康放大——全格挡不仅防伤，还给反杀窗口

### 4.2 连击系统 (Combo)

- Combo 是最大 1 层的 Buff，不可叠加
- **施加**: Strike_L (轻击) —— 攻击敌人时施加 1 层 Combo
- **消耗**: Strike_H (重击) —— 消耗 1 层 Combo + 3 帧 → 返还费用（0 费打出）
- **清除**: 回合结束时 Combo 未使用则自动清除

**设计规则**:
- 轻击 x 重击是核心 2 卡 combo。可设计更多"施加 Combo"或"消耗 Combo"的卡
- Combo 消耗卡应有高收益以补偿需要先打轻击的设置成本
- 消耗 Combo = 检查 `player.Creature.GetPower<Combo>() != null`

### 4.3 投技 (Throw)

- 当前投技 CommandGrab: 消耗 6 帧，造成无视防御伤害
- 帧不足时可打出——差值变为负帧
- `ValueProp.Unblockable` 标记

**设计规则**:
- 投技 = 高伤害 + 高帧消耗 + 无防御。典型的"花帧换血"
- 设计新投技时：帧消耗 ≥ 4，伤害应高于同费攻击牌 50-100%

---

## 5. 数值设计基准（平衡参考）

以 STS2 标准卡为参考系：

| 费用 | 攻击基准伤害 | 技能基准格挡 | 力量/敏捷 | 抽牌 |
|------|------------|------------|----------|------|
| 0 | 4 (+3) | 4 (+2) | — | — |
| 1 | 6 (+3) | 5 (+3) | +1 力/敏 = 1 费 | 1 费抽 2 |
| 2 | 12 (+4) | 11 (+3) | +1 力+1 敏 = 2 费 | — |
| 3 | 18 (+4) | — | — | — |

### 帧数换算

| 帧数效果 | 换算 |
|----------|------|
| 获得 2 帧 | ≈ 0.5 费（条件性，需攻击进攻敌人） |
| 获得 4 帧（无条件） | ≈ 1 费 |
| 消耗 3 帧（连击） | ≈ 1 费折扣 |
| 消耗 6 帧（投技） | ≈ 2 费折扣 → 伤害翻倍 |

### 斗气换算

| 斗气效果 | 换算 |
|----------|------|
| 消耗 2 层斗气 | ≈ 1 费 |
| 有斗气 = 常驻 +1 力 +1 敏 | ≈ 2 费/回合（但受 burnout 限制） |

### 卡牌稀有度加成

- **Common**: 基准数值
- **Uncommon**: 基准 + 额外效果 ≈ +0.5-1 费价值
- **Rare**: 基准 + 强力效果 ≈ +1.5-2 费价值

---

## 6. 卡牌设计模式（模板）

所有卡牌继承 `FighterCardModel`（抽象基类，处理了卡图路径）。必须标注 `[Pool(typeof(FighterCardPool))]`。

### 6.1 攻击牌模板

```csharp
[Pool(typeof(FighterCardPool))]
public sealed class CardName : FighterCardModel
{
    public override List<(string, string)>? Localization => [
        ("title", "卡名"),
        ("description", "造成 [red]{Damage}[/red] 点伤害。\n【其他效果描述】"),
        ("smartDescription", "造成 [red]{Damage}[/red] 点伤害。")
    ];

    public CardName() : base(
        1,                    // 费用 (0-4)
        CardType.Attack,      // 类型
        CardRarity.Common,    // 稀有度 (Basic/Common/Uncommon/Rare)
        TargetType.AnyEnemy   // 目标
    )
    {
        WithDamage(6, 3);     // 基础伤害, 升级增量
        WithTags(CardTag.Strike);
        // WithKeywords(FighterKeywords.Heavy);  // 如需关键字
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 帧数操作
        // 2. 其他效果
        // 3. 伤害
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
```

### 6.2 技能牌模板

```csharp
public CardName() : base(
    1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    WithBlock(5, 3);
    // 其他效果配置
}
```

### 6.3 能力牌模板

```csharp
public CardName() : base(
    2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    // 能力牌不 WithDamage/WithBlock
    // 在 OnPlay 中施加 Power
}
```

---

## 7. 设计约束与检查清单

### 7.1 必须遵守

- [ ] 本地化 key 格式: `FIGHTER-CLASSNAME`（破折号，不是下划线）
- [ ] 类名 = 本地化 key 的后半部分（如 `Strike_L` → `FIGHTER-Strike_L`）
- [ ] 文件放在正确的目录: `Cards/Basic/`, `Cards/Common/`, `Cards/Uncommon/`, `Cards/Rare/`
- [ ] namespace 格式: `Fighter.Code.Cards.{Rarity}`
- [ ] 卡牌类必须标注 `[Pool(typeof(FighterCardPool))]`
- [ ] 所有卡牌必须实现 `Localization` 属性（至少 title + description）

### 7.2 帧数交互卡的设计约束

- 需要正帧才能打出的卡: 在 `OnPlay` 开头检查 `CounterHitState.CanAffordFrameCost(Owner, cost)`
- 需要斗气才能打出的卡: 实现 Harmony patch 类似 `CardPlayabilityPatch`（在 `CanPlay` 中返回 false）
- 消耗 Combo 的卡: 先检查 `GetPower<Combo>() != null`
- 施加 Combo 的卡: 先检查是否已有 Combo，避免无效重复施加

### 7.3 安全写法

- 始终用 `Owner.Creature.GetPower<T>()` 获取 Power，返回 null 时安全处理
- 修改 Power 数量用 `PowerCmd.ModifyAmount`，施加/移除用 `PowerCmd.Apply` / `PowerCmd.Remove`
- 操作帧数前不一定要检查正负值——`PowerCmd.ModifyAmount` 可以安全减到负数

---

## 8. 已实现卡牌列表（当前 5 张）

### 初始卡组 (Basic x5)
| 卡名 | 费 | 类 | 描述 | 机制标签 |
|------|----|----|------|----------|
| 防御 Defend_F | 1 | Skill | 5(+3) Block | Defend tag |
| 轻击 Strike_L | 0 | Attack | 4(+3) dmg | 打进攻敌人+2帧, 施加 Combo |
| 重击 Strike_H | 2 | Attack | 8(+3) dmg | 连击: 0费, 消耗 Combo+3帧 |
| 投技 CommandGrab | 2 | Attack | 13(+4) 穿防 | 消耗 6 帧 (可透支) |
| 绿冲 DriveRush | 0 | Skill | +4(+2) 帧 | 消耗 2 斗气 |

---

## 9. 未实现的卡牌设计空间（给 AI 的创意引导）

以下是推荐的设计方向，AI 可以据此生成新卡：

### 9.1 帧数操作卡
- **帧数生成**: 通过特定条件获得帧（"若敌人未攻击则+4帧"）
- **帧数消费**: 消耗帧换取大伤害/格挡/抽牌/力量
- **帧数清空**: 消耗全部帧的超级效果（"消耗所有帧，每帧造成 2 伤害"）
- **负帧利用**: 负帧时效果增强（"若帧数为负，额外造成伤害"）

### 9.2 连击系统扩展
- **新连击起点卡**: 施加 Combo 的同时附带其他效果（格挡/抽牌/虚弱）
- **新连击终点卡**: 消耗 Combo 的技能牌（不只是攻击）、消耗 Combo 的 AOE
- **多个连击选项**: 玩家有 Combo 时可选择不同路线的终结卡

### 9.3 斗气系统扩展
- **斗气爆发**: 一次性消耗大量（3+）斗气获取强力效果
- **斗气转化**: "每消耗一层斗气，获得 2 点格挡"
- **Burnout 互动**: "若处于 burnout，伤害翻倍" —— 高风险高回报

### 9.4 超杀技 (预留，需 SuperGauge ≥ 1)
- CA1 (1 段): Rare 攻击，伤害 20+，消耗 1 段超杀槽
- CA2 (2 段): Rare 攻击，AOE 或附加强力 debuff，消耗 2 段
- CA3 (3 段): 传说级攻击，消耗 3 段全满，游戏内最强大招

### 9.5 关键字扩展
- **Heavy** 卡的共通机制: 当前 Heavy 仅标签。可实现"打出 Heavy 卡后获得 X"
- **Throw** 投技扩展: 统一"消耗帧数 + 穿防"模板，变化帧消耗和伤害比例

### 9.6 能力牌 (Power) 设计空间
- **帧数相关**: "每回合获得 2 帧" / "正帧时额外抽牌"
- **打康相关**: "打康伤害 +1% 每层力量"
- **连击相关**: "连击卡伤害 +3"
