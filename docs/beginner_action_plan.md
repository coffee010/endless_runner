# 新手行动计划

这份文档给没有 Unity 经验的同学看。先按顺序做，不要在占位玩家原型跑通之前去抠美术、模型和复杂 Shader。

## 当前进度

仓库里已经完成：

- Git 仓库已初始化。
- GitHub 远程仓库已配置。
- 项目规划文档已添加。
- 三人分工文档已添加。
- 第一批 C# 脚本已经放在 `Assets/Scripts` 下。
- Unity URP 工程已经创建到仓库根目录。
- 已经有 `Packages/` 和 `ProjectSettings/`。
- 当前 Unity Editor 版本是 Unity 6 `6000.4.7f1`。

还没有完成：

- 正式主场景 `Assets/Scenes/Main.unity` 还没有创建。
- prefab、材质、UI 还没有在 Unity 里创建。
- 场景对象还没有挂好脚本和 Inspector 引用。

## 第一目标

48 小时内先做出这个原型：

- 占位胶囊体玩家自动向前跑。
- A/D 可以左右换道。
- Space 可以跳跃。
- S 可以下滑。
- 简单路段可以不断生成。
- 撞到障碍会失败。
- 按 R 可以重开。
- 分数会增加。

这个版本跑通，项目就可行。

## 同学 1 先做什么

负责人：同学 1

### 第 1 步：安装 Unity

用 Unity Hub 安装：

- Unity Editor：Unity 6 `6000.4.7f1`
- 团队成员尽量安装同一个版本。不要用 `2022.3 LTS` 打开这个项目，因为项目已经由 Unity 6 创建，降版本打开更容易出兼容问题。
- 后面创建项目用的模板：`Universal 3D` 或 `3D (URP)`
- 必装模块：`Windows Build Support`
- IDE：如果你电脑上没有 Visual Studio 或 Rider，就装 `Visual Studio Community`

现在不需要装 Android、WebGL、iOS、Linux 模块。

### 第 2 步：创建 Unity 项目

优先尝试直接创建到这个目录：

```text
D:\programing\code\cghw\endless_runner
```

项目设置：

- Template：`Universal 3D` 或 `3D (URP)`
- Project name：`endless_runner` 或 `NeonRush`

如果 Unity Hub 提示这个文件夹不是空的，不允许创建：

1. 先在临时文件夹创建 Unity 项目。
2. 关闭 Unity。
3. 把 Unity 生成的这些内容移动到当前仓库：
   - `Assets`
   - `Packages`
   - `ProjectSettings`
   - `.sln` 和 `.csproj` 文件可以留着，Git 会忽略它们。
4. 保留仓库里已有的这些内容：
   - `.git`
   - `.gitignore`
   - `README.md`
   - `docs`
   - `game.txt`

### 第 3 步：打开 Unity，等待导入完成

第一次打开项目可能要导入几分钟。等 Unity 停止编译。

如果 Console 出现红色报错，先截图发出来，不要乱改设置。

### 第 4 步：创建主场景

在 Unity 里：

1. 创建文件夹 `Assets/Scenes`。
2. 保存当前场景为：

```text
Assets/Scenes/Main.unity
```

场景里创建这些对象：

- `GameManager`
- `TrackSpawner`
- `Player`
- `Main Camera`
- `Directional Light`
- `Global Volume`
- `Canvas`

### 第 5 步：创建占位玩家

1. 菜单选择 `GameObject > 3D Object > Capsule`。
2. 改名为 `Player`。
3. Position 设置为 `(0, 1, 0)`。
4. 添加组件：
   - `Character Controller`
   - `RunnerController`
   - `PlayerCollision`
   - `EnergyModeController`
   - `EnergyBurst`
5. 在 `Character Controller` 里设置：
   - Height：`2`
   - Radius：`0.4`
   - Center Y：`1`
6. 在 `EnergyBurst` 组件里：
   - 把 `Player` 自己拖到 `Runner` 字段。

### 第 6 步：设置 GameManager

1. 创建空物体 `GameManager`。
2. 添加组件：
   - `GameManager`
   - `ScoreManager`
3. 在 `ScoreManager` 里：
   - 把 `Player` 拖到 `Runner` 字段。

### 第 7 步：设置 TrackSpawner

1. 创建空物体 `TrackSpawner`。
2. 添加组件：
   - `TrackSpawner`
3. 把 `Player` 拖到 `Player` 字段。

现在它还需要同学 2 做一个路段 prefab。

## 按当前进度分配任务

## 同学 1：总集成和核心玩法

同学 1 负责：

```text
Assets/Scenes
Assets/Scripts/Core
Assets/Scripts/Player
Assets/Scripts/UI
```

今天和明天要做：

- 安装 Unity。
- 创建 URP 项目。
- 打开项目，确保没有编译错误。
- 创建 `Main.unity`。
- 创建占位玩家。
- 挂载脚本。
- 确保可以进入 Play Mode。
- Unity Console 有红色报错就发给我。

暂时不要花时间做：

- 最终角色模型。
- 好看的大场景。
- 复杂 Shader。

胶囊体不是“最终方案”，只是最快验证玩法的占位物。想做得更好，通常要多花时间在角色模型、动画、碰撞体适配和表现细节上，所以前期先别把时间消耗在这里。

同学 1 的验收标准：

- 按 Play 后，占位玩家能自动向前跑。

## 同学 2：路段和 prefab

同学 2 负责：

```text
Assets/Prefabs/TrackSegments
Assets/Prefabs/Obstacles
Assets/Prefabs/Collectibles
Assets/Scripts/Track
Assets/Scripts/Gameplay
```

今天和明天要做：

- 等 Unity 项目创建好以后开始做。
- 创建文件夹：
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
- 做一个测试路段 prefab：
  - 根物体叫 `TrackSegment_Test`
  - 根物体添加 `TrackSegment` 脚本
  - 添加一个 Cube 子物体作为地面
  - 地面 Position：`(0, -0.05, 15)`
  - 地面 Scale：`(8, 0.1, 30)`
  - 保存到 `Assets/Prefabs/TrackSegments`
- 做一个障碍 prefab：
  - 用 Cube
  - 添加 `Obstacle`
  - 保存到 `Assets/Prefabs/Obstacles`
- 做一个收集物 prefab：
  - 用 Sphere
  - Collider 勾选 `Is Trigger`
  - 添加 `Collectible`
  - 保存到 `Assets/Prefabs/Collectibles`

暂时不要花时间做：

- 一口气做 6 个精致路段。
- 导入复杂模型。
- 复杂随机生成。

同学 2 的验收标准：

- 路段 prefab 可以被拖进 `TrackSpawner > Segment Prefabs` 列表。

## 同学 3：视觉和报告

同学 3 负责：

```text
Assets/Art
Assets/Materials
Assets/Shaders
Assets/VFX
Assets/Prefabs/VFX
docs/asset_sources.md
docs/graphics_notes.md
```

今天和明天要做：

- 创建文件夹：
  - `Assets/Materials`
  - `Assets/VFX`
  - `Assets/Art`
  - `Assets/Shaders`
- 创建占位材质：
  - `M_Track_Neon`
  - `M_Obstacle_Neon`
  - `M_Collectible_Energy`
- 先用 URP/Lit 材质。
- 开启 Emission：
  - 跑道：青色或蓝色
  - 障碍：红色或紫红色
  - 收集物：青色或绿色
- 帮忙在 `Global Volume` 里开启 Bloom。
- 如果用了外部资源，开始填写 `docs/asset_sources.md`。

暂时不要花时间做：

- 在原型跑通前做最终 Shader Graph。
- 找素材找几个小时。
- 替换正式角色模型。

同学 3 的验收标准：

- 测试场景看起来是暗色 + 霓虹，而不是 Unity 默认灰色。

## 小组 Git 规则

开始工作前：

```powershell
git pull
```

完成一个可运行改动后：

```powershell
git status
git add .
git commit -m "简短描述这次改动"
git push
```

不要提交这些目录：

- `Library`
- `Temp`
- `Obj`
- `Logs`
- `Build`

这些已经在 `.gitignore` 里忽略了。

## 发给我什么最有用

遇到问题时，发这些给我：

- Unity Console 红色报错截图或文字。
- Inspector 里不知道怎么填的字段截图。
- Git 报错文字。
- 脚本不生效时的 Hierarchy 截图。

最有用的截图是同时包含：

- Unity Console
- 当前选中物体的 Inspector
- 左边 Hierarchy

## 不用慌的情况

这些都是正常的：

- Unity 第一次导入要几分钟。
- 出现很多 `.meta` 文件。
- Visual Studio 自动打开。
- 第一版原型只有胶囊体和方块。
- 前几天场景看起来很丑。

这些不正常，要尽快处理：

- 项目有编译错误。
- 不能进入 Play Mode。
- 没人负责 prefab。
- 三个人同时改同一个场景。
