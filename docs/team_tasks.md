# Team Task Split

This document is the working split for the 3-person Neon Rush project. The goal is to reduce merge conflicts and make every teammate responsible for clear Unity folders.

## Roles

| Role | Owner | Main Responsibility | Primary Folders |
| --- | --- | --- | --- |
| Lead Integrator / Gameplay | 你 | Project integration, scene wiring, core gameplay scripts, UI logic, GitHub coordination | `Assets/Scripts/Core`, `Assets/Scripts/Player`, `Assets/Scripts/UI`, `Assets/Scenes` |
| Track / Prefab Builder | 同学 1 | Track segment prefabs, obstacle prefabs, collectible prefabs, placement rules | `Assets/Prefabs/TrackSegments`, `Assets/Prefabs/Obstacles`, `Assets/Prefabs/Collectibles`, `Assets/Scripts/Track`, `Assets/Scripts/Gameplay` |
| Graphics / Assets / Presentation | 同学 2 | External assets, materials, shaders, particles, post-processing, screenshots, report assets | `Assets/Art`, `Assets/Materials`, `Assets/Shaders`, `Assets/VFX`, `Assets/Prefabs/VFX`, `docs` |

## Lead Integrator / Gameplay

Owner: 你

### Must Finish

- Create and maintain the main scene:
  - `Assets/Scenes/Main.unity`
- Create scene root objects:
  - `GameManager`
  - `TrackSpawner`
  - `Player`
  - `Main Camera`
  - `Global Volume`
  - `Canvas`
- Implement or integrate core scripts:
  - `Assets/Scripts/Core/GameManager.cs`
  - `Assets/Scripts/Core/ScoreManager.cs`
  - `Assets/Scripts/Player/RunnerController.cs`
  - `Assets/Scripts/Player/PlayerCollision.cs`
  - `Assets/Scripts/UI/UIManager.cs`
- Wire scripts to objects in the scene.
- Own the game loop:
  - start game
  - running
  - game over
  - restart
- Own input:
  - A/D or arrow keys for lane switch
  - Space/W for jump
  - S for slide
  - R for restart
- Own Git:
  - check merge conflicts
  - verify the project opens
  - make milestone commits
  - push to GitHub

### Should Not Own

- Do not hand-place all track obstacles yourself unless teammate 1 is blocked.
- Do not spend too long polishing particles/materials unless teammate 2 is blocked.

### Definition Of Done

- A placeholder player can run for at least 60 seconds.
- Collision with an obstacle causes game over.
- Restart works.
- Score and energy UI update.
- Teammate prefabs can be dragged into the scene without script errors.

## Track / Prefab Builder

Owner: 同学 1

### Must Finish

- Create these folders after Unity project exists:
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
- Create 6 fixed-length track segment prefabs:
  - `TrackSegment_Straight_01.prefab`
  - `TrackSegment_Collectibles_01.prefab`
  - `TrackSegment_LowObstacle_01.prefab`
  - `TrackSegment_HighObstacle_01.prefab`
  - `TrackSegment_LaneBlock_01.prefab`
  - `TrackSegment_ColorGate_01.prefab`
- Keep every segment length the same:
  - recommended length: `30`
  - segment start at local `Z = 0`
  - segment end at local `Z = 30`
- Create obstacle prefabs:
  - `Obstacle_Low.prefab`
  - `Obstacle_High.prefab`
  - `Obstacle_LaneBlock.prefab`
  - `Gate_Color_Red.prefab`
  - `Gate_Color_Blue.prefab`
  - `Gate_Color_Green.prefab`
- Create collectible prefabs:
  - `Collectible_Energy.prefab`
  - `Collectible_Score.prefab`
- Work with these scripts:
  - `Assets/Scripts/Track/TrackSegment.cs`
  - `Assets/Scripts/Track/TrackSpawner.cs`
  - `Assets/Scripts/Gameplay/Obstacle.cs`
  - `Assets/Scripts/Gameplay/Collectible.cs`
  - `Assets/Scripts/Gameplay/ColorGate.cs`

### Placement Rules

- Use three lane X positions only:
  - left: `-2.5`
  - center: `0`
  - right: `2.5`
- Avoid impossible patterns:
  - never block all 3 lanes at the same Z position
  - do not place high obstacle and low obstacle too close together
  - keep at least `5` units between major obstacles
- Use simple primitive shapes first:
  - cube for obstacles
  - capsule or plane for gates
  - sphere for energy collectibles
- Prefabs should work before final art is imported.

### Should Not Own

- Do not edit `RunnerController.cs` or `GameManager.cs` unless agreed.
- Do not change global post-processing or shader files unless agreed with teammate 2.

### Definition Of Done

- At least 6 track segment prefabs exist.
- Each segment connects cleanly with no visible offset.
- Obstacles and collectibles have colliders/triggers configured.
- The lead integrator can add the segment prefabs to `TrackSpawner` and run the game.

## Graphics / Assets / Presentation

Owner: 同学 2

### Must Finish

- Create and maintain asset folders:
  - `Assets/Art/Characters`
  - `Assets/Art/Environment`
  - `Assets/Art/Obstacles`
  - `Assets/Art/Textures`
  - `Assets/Materials`
  - `Assets/Shaders`
  - `Assets/VFX`
  - `Assets/Prefabs/VFX`
- Record external assets in:
  - `docs/asset_sources.md`
- Create minimum materials:
  - `M_Track_Neon.mat`
  - `M_Obstacle_Neon.mat`
  - `M_Gate_Red.mat`
  - `M_Gate_Blue.mat`
  - `M_Gate_Green.mat`
  - `M_Collectible_Energy.mat`
- Create minimum shaders or Shader Graphs:
  - `Assets/Shaders/NeonTrack`
  - `Assets/Shaders/EnergyGate`
  - optional: `Assets/Shaders/Dissolve`
- Create minimum VFX prefabs:
  - `VFX_Collect.prefab`
  - `VFX_Burst.prefab`
  - `VFX_Hit.prefab`
  - `VFX_GatePass.prefab`
- Configure the main visual stack:
  - Bloom
  - Color Adjustments
  - Vignette
  - neon emission colors

### Report / Defense Materials

- Capture screenshots for:
  - gameplay overview
  - neon track material
  - energy gate material
  - particles
  - Bloom on/off comparison
- Maintain a short notes file:
  - `docs/graphics_notes.md`
- Prepare report sections:
  - external asset source explanation
  - shader effect explanation
  - post-processing explanation
  - before/after screenshots

### Should Not Own

- Do not change track prefab placement rules without teammate 1.
- Do not change gameplay state scripts without the lead integrator.

### Definition Of Done

- The scene has a clear neon sci-fi look.
- Bloom and emission are visible in screenshots.
- At least one shader effect is easy to explain in the report.
- Asset sources are recorded with URL and license.

## Codex Support

I will mainly help with:

- Writing C# scripts.
- Writing or designing Shader Graph/HLSL effects.
- Debugging Unity errors.
- Designing prefab interfaces.
- Explaining what to attach in Inspector.
- Writing report technical descriptions.
- Reviewing Git changes before merging.

The team still needs to do:

- Unity Editor dragging and Inspector assignment.
- Prefab construction.
- Importing art/audio assets.
- Visual tuning.
- Local playtesting.
- Screenshots, video and defense rehearsal.

## Folder Ownership Rules

- Lead integrator owns:
  - `Assets/Scenes`
  - `Assets/Scripts/Core`
  - `Assets/Scripts/Player`
  - `Assets/Scripts/UI`
- Teammate 1 owns:
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
  - `Assets/Scripts/Track`
  - `Assets/Scripts/Gameplay`
- Teammate 2 owns:
  - `Assets/Art`
  - `Assets/Materials`
  - `Assets/Shaders`
  - `Assets/VFX`
  - `Assets/Prefabs/VFX`
  - `docs/asset_sources.md`
  - `docs/graphics_notes.md`

If a change touches another teammate's folder, mention it before committing.

## Branch Naming

- Lead integrator:
  - `feature/gameplay-core`
  - `feature/ui-game-loop`
- Teammate 1:
  - `feature/track-prefabs`
  - `feature/obstacles-collectibles`
- Teammate 2:
  - `feature/neon-graphics`
  - `feature/vfx-presentation`

## First 3 Days

### Day 1

- Lead integrator:
  - create Unity URP project
  - create `Main.unity`
  - add placeholder player, camera, GameManager
- Teammate 1:
  - create 2 track segment prefabs with primitive geometry
  - create low obstacle and energy collectible prefabs
- Teammate 2:
  - create placeholder neon materials
  - enable Bloom and Color Adjustments
  - fill first rows in `docs/asset_sources.md`

### Day 2

- Lead integrator:
  - integrate runner movement and restart loop
  - add score UI
- Teammate 1:
  - expand to 6 track segment prefabs
  - add high obstacle and lane block obstacle
- Teammate 2:
  - make track/gate materials visible and consistent
  - create collect and hit particles

### Day 3

- Lead integrator:
  - integrate track spawning and collision
  - verify one-minute playable loop
- Teammate 1:
  - fix impossible obstacle layouts
  - test segment connection
- Teammate 2:
  - take first screenshots
  - document shader/post-processing plan

## MVP Lock

Do not expand scope until these are done:

- Player can run, switch lanes, jump and slide.
- Track segments spawn continuously.
- Obstacles can cause failure.
- Collectibles update score or energy.
- Game over and restart work.
- Neon material, Bloom and at least one particle effect are visible.
