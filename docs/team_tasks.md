# 三人任务分工

这份文档是《Neon Rush：霓虹轨道》的正式分工。目标是减少合并冲突，让每个人负责清楚的 Unity 目录。

说明：第一版玩家可以先用 Capsule 胶囊体。它不是最终角色，只是为了最快验证跑酷玩法、碰撞和镜头。更好的角色表现需要模型、动画和碰撞调试，会明显增加时间成本，放到原型跑通后再做。

## 角色总览

| 角色 | 负责人 | 主要职责 | 主要目录 |
| --- | --- | --- | --- |
| 总集成 / 核心玩法 | 同学 1 | 项目集成、主场景搭建、核心玩法脚本、UI 逻辑、Git 协调 | `Assets/Scripts/Core`, `Assets/Scripts/Player`, `Assets/Scripts/UI`, `Assets/Scenes` |
| 路段 / Prefab 搭建 | 同学 2 | 路段 prefab、障碍 prefab、收集物 prefab、摆放规则 | `Assets/Prefabs/TrackSegments`, `Assets/Prefabs/Obstacles`, `Assets/Prefabs/Collectibles`, `Assets/Scripts/Track`, `Assets/Scripts/Gameplay` |
| 图形 / 资源 / 展示 | 同学 3 | 外部资源、材质、Shader、粒子、后处理、截图、报告素材 | `Assets/Art`, `Assets/Materials`, `Assets/Shaders`, `Assets/VFX`, `Assets/Prefabs/VFX`, `docs` |

## 同学 1：总集成 / 核心玩法

负责人：同学 1

### 必须完成

- 创建并维护主场景：
  - `Assets/Scenes/Main.unity`
- 创建场景根对象：
  - `GameManager`
  - `TrackSpawner`
  - `Player`，第一版可以用 Capsule 占位
  - `Main Camera`
  - `Global Volume`
  - `Canvas`
- 集成核心脚本：
  - `Assets/Scripts/Core/GameManager.cs`
  - `Assets/Scripts/Core/ScoreManager.cs`
  - `Assets/Scripts/Player/RunnerController.cs`
  - `Assets/Scripts/Player/PlayerCollision.cs`
  - `Assets/Scripts/UI/UIManager.cs`
- 把脚本挂到场景对象上。
- 负责游戏流程：
  - 开始
  - 运行
  - 失败
  - 重开
- 负责输入：
  - A/D 或方向键左右换道
  - Space/W 跳跃
  - S 下滑
  - R 重开
- 负责 Git 集成：
  - 检查合并冲突
  - 确认项目能打开
  - 做阶段性 commit
  - 最终手动 push 到 GitHub

### 不应该主要负责

- 不要自己把所有障碍都摆完，除非同学 2 卡住。
- 不要长时间调粒子和材质，除非同学 3 卡住。

### 验收标准

- 占位玩家可以连续跑至少 60 秒。
- 撞到障碍会 Game Over。
- 重开功能正常。
- 分数和能量 UI 会更新。
- 同学做的 prefab 可以拖进场景或 `TrackSpawner`，没有脚本报错。

## 同学 2：路段 / Prefab 搭建

负责人：同学 2

### 必须完成

- Unity 项目创建后，建立这些文件夹：
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
- 创建 6 个固定长度路段 prefab：
  - `TrackSegment_Straight_01.prefab`
  - `TrackSegment_Collectibles_01.prefab`
  - `TrackSegment_LowObstacle_01.prefab`
  - `TrackSegment_HighObstacle_01.prefab`
  - `TrackSegment_LaneBlock_01.prefab`
  - `TrackSegment_ColorGate_01.prefab`
- 每个路段长度保持一致：
  - 推荐长度：`30`
  - 路段起点在本地坐标 `Z = 0`
  - 路段终点在本地坐标 `Z = 30`
- 创建障碍 prefab：
  - `Obstacle_Low.prefab`
  - `Obstacle_High.prefab`
  - `Obstacle_LaneBlock.prefab`
  - `Gate_Color_Red.prefab`
  - `Gate_Color_Blue.prefab`
  - `Gate_Color_Green.prefab`
- 创建收集物 prefab：
  - `Collectible_Energy.prefab`
  - `Collectible_Score.prefab`
- 配合这些脚本工作：
  - `Assets/Scripts/Track/TrackSegment.cs`
  - `Assets/Scripts/Track/TrackSpawner.cs`
  - `Assets/Scripts/Gameplay/Obstacle.cs`
  - `Assets/Scripts/Gameplay/Collectible.cs`
  - `Assets/Scripts/Gameplay/ColorGate.cs`

### 摆放规则

- 只使用三条轨道的 X 坐标：
  - 左：`-2.5`
  - 中：`0`
  - 右：`2.5`
- 避免无解组合：
  - 同一个 Z 位置不要挡住三条轨道。
  - 高障碍和低障碍不要贴得太近。
  - 主要障碍之间至少间隔 `5` 个单位。
- 第一版先用简单几何体：
  - Cube 做障碍
  - Capsule 或 Plane 做能量门
  - Sphere 做能量收集物
- prefab 先能用，再替换最终美术。

### 不应该主要负责

- 不要改 `RunnerController.cs` 或 `GameManager.cs`，除非和同学 1 商量过。
- 不要改全局后处理和 Shader 文件，除非和同学 3 商量过。

### 验收标准

- 至少有 6 个路段 prefab。
- 每个路段能无缝连接，没有明显错位。
- 障碍和收集物的 Collider / Trigger 配置正确。
- 同学 1 可以把这些路段 prefab 加到 `TrackSpawner` 后直接运行。

## 同学 3：图形 / 资源 / 展示

负责人：同学 3

### 必须完成

- 创建并维护资源目录：
  - `Assets/Art/Characters`
  - `Assets/Art/Environment`
  - `Assets/Art/Obstacles`
  - `Assets/Art/Textures`
  - `Assets/Materials`
  - `Assets/Shaders`
  - `Assets/VFX`
  - `Assets/Prefabs/VFX`
- 记录外部资源来源：
  - `docs/asset_sources.md`
- 创建最小可用材质：
  - `M_Track_Neon.mat`
  - `M_Obstacle_Neon.mat`
  - `M_Gate_Red.mat`
  - `M_Gate_Blue.mat`
  - `M_Gate_Green.mat`
  - `M_Collectible_Energy.mat`
- 创建最小可用 Shader 或 Shader Graph：
  - `Assets/Shaders/NeonTrack`
  - `Assets/Shaders/EnergyGate`
  - 可选：`Assets/Shaders/Dissolve`
- 创建最小可用 VFX prefab：
  - `VFX_Collect.prefab`
  - `VFX_Burst.prefab`
  - `VFX_Hit.prefab`
  - `VFX_GatePass.prefab`
- 配置主要画面效果：
  - Bloom
  - Color Adjustments
  - Vignette
  - 霓虹 Emission 颜色

### 报告 / 答辩材料

- 截图内容：
  - 游戏整体画面
  - 霓虹轨道材质
  - 能量门材质
  - 粒子效果
  - Bloom 开关对比
- 维护一份简短图形说明：
  - `docs/graphics_notes.md`
- 准备报告部分：
  - 外部资源来源说明
  - Shader 效果说明
  - 后处理说明
  - 前后对比截图

### 不应该主要负责

- 不要擅自改变同学 2 的路段 prefab 摆放规则。
- 不要擅自改核心玩法状态脚本。

### 验收标准

- 场景有明确的霓虹科幻风格。
- 截图里能明显看到 Bloom 和发光材质。
- 至少有一个 Shader 效果可以在报告里讲清楚。
- 外部资源都有 URL 和 License 记录。

## Codex 辅助范围

我主要帮你们做：

- 写 C# 脚本。
- 写或设计 Shader Graph / HLSL 效果。
- Debug Unity 报错。
- 设计 prefab 接口。
- 说明 Inspector 里该挂什么。
- 写报告技术说明。
- 合并前帮你们看改动风险。

你们仍然需要自己做：

- Unity Editor 里的拖拽和 Inspector 赋值。
- prefab 搭建。
- 导入美术和音频资源。
- 视觉调参。
- 本机 Play Mode 测试。
- 截图、录屏和答辩演练。

## 目录归属规则

- 同学 1 负责：
  - `Assets/Scenes`
  - `Assets/Scripts/Core`
  - `Assets/Scripts/Player`
  - `Assets/Scripts/UI`
- 同学 2 负责：
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
  - `Assets/Scripts/Track`
  - `Assets/Scripts/Gameplay`
- 同学 3 负责：
  - `Assets/Art`
  - `Assets/Materials`
  - `Assets/Shaders`
  - `Assets/VFX`
  - `Assets/Prefabs/VFX`
  - `docs/asset_sources.md`
  - `docs/graphics_notes.md`

如果一次改动碰到别人的目录，commit 前先说一声。

## 建议分支名

- 同学 1：
  - `feature/gameplay-core`
  - `feature/ui-game-loop`
- 同学 2：
  - `feature/track-prefabs`
  - `feature/obstacles-collectibles`
- 同学 3：
  - `feature/neon-graphics`
  - `feature/vfx-presentation`

## 前 3 天安排

### 第 1 天

- 同学 1：
  - 创建 Unity URP 项目
  - 创建 `Main.unity`
  - 添加 Capsule 占位玩家、相机、GameManager
- 同学 2：
  - 用基础几何体创建 2 个路段 prefab
  - 创建低障碍和能量收集物 prefab
- 同学 3：
  - 创建占位霓虹材质
  - 开启 Bloom 和 Color Adjustments
  - 填写 `docs/asset_sources.md` 的第一批资源来源

### 第 2 天

- 同学 1：
  - 集成玩家移动和重开流程
  - 添加分数 UI
- 同学 2：
  - 扩展到 6 个路段 prefab
  - 添加高障碍和轨道阻挡障碍
- 同学 3：
  - 统一跑道和能量门材质
  - 创建收集粒子和撞击粒子

### 第 3 天

- 同学 1：
  - 集成路段生成和碰撞
  - 验证可以连续跑 1 分钟
- 同学 2：
  - 修掉无解障碍组合
  - 测试路段接缝
- 同学 3：
  - 截第一批效果图
  - 写 Shader / 后处理方案说明

## MVP 锁定规则

下面这些完成前，不要扩展范围：

- 玩家可以跑、换道、跳跃、下滑。
- 路段可以连续生成。
- 障碍可以导致失败。
- 收集物可以更新分数或能量。
- Game Over 和 Restart 正常。
- 霓虹材质、Bloom、至少一个粒子效果可见。
