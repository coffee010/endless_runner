# 组员分工文档

本文档记录《Neon Rush：霓虹轨道》的最终中文分工、目录归属和素材责任。项目统一使用 Unity 6 `6000.4.7f1`，主场景为 `Assets/Scenes/Main.unity`。

## 分工总览

| 成员 | 主要职责 | 相关目录/内容 |
| --- | --- | --- |
| 同学 1（队长） | 游戏主要逻辑、项目整合、赛博整体风格、后处理、环境模型搭建、渲染包导入、游戏音效、3D 角色模型导入 | `Assets/Scripts/Core`、`Assets/Scripts/Player`、`Assets/Scripts/UI`、`Assets/Scenes`、`Assets/Settings`、`Assets/ThirdParty`、`Assets/Naxida`、`Assets/Audio` |
| 同学 2 | 跑道部分，包括跑道段、障碍物、收集物制作，以及它们在跑道上的摆放和调试 | `Assets/Scripts/Track`、`Assets/Scripts/Gameplay`、`Assets/Prefabs/TrackSegments` |
| 同学 3 | 能量爆发特效、游戏开始界面、跑道光效、视觉展示内容 | `Assets/Scripts/Gameplay/EnergyBurst.cs`、`Assets/Scripts/Graphics`、`Assets/Prefabs/VFX`、`Assets/Shades`、`docs/graphics_notes.md` |

## 同学 1（队长）

### 主要工作

- 编写和整合游戏主要逻辑：
  - 游戏开始、运行、失败、重新开始流程
  - 分数、速度、能量等核心状态
  - 玩家控制和动画状态衔接
  - UI 管理和主场景对象连接
- 搭建项目主场景和基础对象：
  - `GameManager`
  - `TrackSpawner`
  - `Player`
  - `Main Camera`
  - `Global Volume`
  - `Canvas`
  - `MusicManager`
- 负责整体赛博风格：
  - Bloom、Color Adjustments、Vignette、Fog 等后处理
  - 赛博环境模型摆放和整体视觉气氛
  - 场景可读性和演示画面调整
- 负责外部资源导入和集成：
  - 导入并配置渲染相关包 `DELTation Toon Shader`
  - 导入 Quaternius Cyberpunk Game Kit 环境模型并用于赛博跑道和环境搭建
  - 从网络导入原神纳西妲/Nahida 相关 3D 人物模型，作为玩家角色
  - 导入 Starter Assets，用于基础动画、动画控制器和可选脚步/落地音效资源
  - 配置游戏背景音乐和音效相关对象

### 网络/第三方来源标注

- 角色模型来源记录为 APlayBox：`https://www.aplaybox.com/`
- 纳西妲/Nahida 角色版权归《原神》及米哈游 miHoYo/HoYoverse 所有，本项目仅作学习展示。
- 环境模型来源为 Quaternius Ultimate Platformer Pack / Cyberpunk Game Kit：`https://quaternius.com/`
- 渲染包为 DELTation Toon Shader：`https://github.com/DELTAation/toon-shader`
- 赛博后处理和 Bloom 风格参考 Delt06 URP Toon Shader Cyberpunk Demo：`https://github.com/Delt06/urp-toon-shader-cyberpunk-demo`
- Starter Assets 来源为 Unity Asset Store：`https://assetstore.unity.com/packages/essentials/starter-assets-thirdperson-updates-in-new-charactercontroller-pa-196526`
- 背景音乐为 `Assets/Audio/Music/FH6_You.mp3`，标注为《极限竞速：地平线 6 / Forza Horizon 6》相关音乐素材，版权归原权利方所有。

## 同学 2

### 主要工作

- 负责跑道部分制作：
  - 制作跑道段 prefab
  - 制作障碍物
  - 制作收集物
  - 将障碍物和收集物合理放置在跑道上
- 配合跑道生成逻辑：
  - 调整跑道段长度和连接位置
  - 保证跑道段可以连续生成
  - 保证障碍物组合不会出现明显无解情况
- 配置碰撞和触发器：
  - 障碍物 Collider
  - 收集物 Trigger
  - 能量门或特殊玩法对象的触发范围

### 验收重点

- 跑道可以连续生成，没有明显断层或错位。
- 障碍物、收集物在三轨道上摆放清晰。
- 玩家可以正常躲避障碍物、收集物品。
- 障碍物和收集物不会挡住所有路线导致无解。

## 同学 3

### 主要工作

- 负责能量爆发特效：
  - `EnergyBurst` 触发反馈
  - 爆发粒子、光效、范围感和清屏表现
- 负责游戏开始界面：
  - 开始提示
  - 开始状态 UI
  - 和主游戏流程衔接
- 负责跑道光效：
  - 发光跑道边缘
  - 流动霓虹线条
  - 速度线或增强运动感的视觉元素
- 负责视觉说明材料：
  - 整理图形效果说明
  - 准备展示截图
  - 说明 Bloom、Shader、粒子效果的作用

### 验收重点

- 能量爆发触发时有清晰可见的视觉反馈。
- 开始界面能和游戏流程正确衔接。
- 跑道光效能体现赛博风格，同时不影响玩家判断障碍物和能量门。
- 图形展示内容可以用于报告或答辩说明。

## 目录归属

```text
Assets/
  Audio/                      同学 1：背景音乐和音效资源
  Naxida/                     同学 1：玩家角色模型资源
  Scenes/                     同学 1：主场景整合
  Settings/                   同学 1：URP、后处理、渲染设置
  ThirdParty/                 同学 1：第三方环境模型和渲染包相关资源
  Scripts/
    Core/                     同学 1
    Player/                   同学 1
    UI/                       同学 1，同学 3 负责开始界面相关内容
    Track/                    同学 2
    Gameplay/                 同学 2 为主，同学 3 负责能量爆发相关内容
    Graphics/                 同学 3
  Prefabs/
    TrackSegments/            同学 2
    VFX/                      同学 3
  Shades/                     同学 3
docs/
  asset_sources.md            同学 1 维护素材来源和版权说明
  graphics_notes.md           同学 3 维护图形效果说明
  team_tasks.md               全组共同确认分工
```

## 素材和版权说明

- 所有外部素材都应记录在 `docs/asset_sources.md`。
- 背景音乐 `FH6_You.mp3` 只用于课程学习和非商业展示；版权归《极限竞速：地平线 6 / Forza Horizon 6》相关权利方所有。
- 原神纳西妲/Nahida 人物模型只用于学习展示；角色版权归《原神》及米哈游 miHoYo/HoYoverse 所有，模型设计和相关 IP 归原权利方所有。
- 如果项目要公开发布或商用，需要替换所有授权不明确或仅适合学习展示的素材。
