# Neon Rush: 霓虹轨道

三轨道 3D 无尽跑酷 Demo。玩家左右换道、跳跃、下滑，躲避障碍并收集能量；满能量后释放爆发清理前方障碍。项目重点是完整玩法闭环和图形学展示：URP、Bloom、霓虹材质、能量门、溶解效果、粒子反馈和展示模式。

## Recommended Setup

- Unity: 2022.3 LTS 或 2023.2 LTS
- Template: 3D URP
- Render Pipeline: Universal Render Pipeline
- Input: 先用旧 Input Manager，降低实现风险；后期可换 New Input System
- Target: Windows PC 演示包
- Repository branch: `main`

## Minimum Playable Version

- 三轨道自动奔跑
- A/D 或方向键换道
- Space/W 跳跃
- S 下滑
- 障碍、收集物、能量门
- 分数、能量条、失败界面、重开
- 霓虹轨道材质、能量门材质、Bloom、粒子

## Stretch Goals

- 能量爆发清除障碍
- 障碍溶解 Shader
- 速度线 UI overlay 或 URP Full Screen Pass
- 展示模式：开关 Bloom、流光、溶解、速度线

## Folder Layout

Unity 项目创建后，在 `Assets/` 下保持下面结构：

```text
Assets/
  Art/
    Characters/
    Environment/
    Obstacles/
    Textures/
  Materials/
  Prefabs/
    Player/
    TrackSegments/
    Obstacles/
    Collectibles/
    VFX/
    UI/
  Scenes/
  Scripts/
    Core/
    Player/
    Track/
    Gameplay/
    Graphics/
    UI/
  Shaders/
  VFX/
  Audio/
  ThirdParty/
```

## First Scene Objects

- `GameManager`
- `TrackSpawner`
- `Player`
- `Main Camera`
- `Directional Light`
- `Global Volume`
- `Canvas`
- `EventSystem`

## Notes

`game.txt` contains the original planning discussion. Keep external asset licenses and URLs in `docs/asset_sources.md`.

Team responsibilities are defined in `docs/team_tasks.md`.
