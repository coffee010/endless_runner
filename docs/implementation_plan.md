# Neon Rush Implementation Plan

## Decisions Already Made

- Genre: 3D endless runner
- Theme: neon sci-fi track
- Core lanes: 3 lanes
- Movement: lane switch, jump, slide
- Graphics target: URP with Bloom, emission materials, particles, shader-driven effects
- Minimum target: playable classroom demo, not a full commercial game

## Open Decisions To Confirm

1. Unity version: this project uses Unity 6 `6000.4.7f1`; keep the team on the same Editor version unless the teacher requires another version.
2. Team size and schedule: current plan assumes 3 people and about 2 weeks.
3. Required course focus: if the teacher requires HLSL code instead of Shader Graph, we should adjust shader implementation.
4. Asset policy: confirm whether Unity Asset Store, Mixamo, Sketchfab, Kenney, Freesound are allowed.
5. Final delivery: confirm whether you need Windows build, source project, report, PPT, or video.

## Milestone 1: Project Bootstrap

- Create Unity 3D URP project inside this repository.
- Create `Assets/` folder layout from README.
- Create `Scenes/Main.unity`.
- Configure URP asset, camera, Global Volume, Bloom and Color Adjustments.
- Add placeholder player cube/capsule, track floor, obstacle cubes and collectable spheres.

Deliverable: a scene that opens cleanly and can enter Play Mode.

## Milestone 2: Playable Core

Scripts:

- `RunnerController.cs`: forward speed, lane switching, jump, slide.
- `PlayerCollision.cs`: obstacle collision, collectible trigger, gate trigger.
- `GameManager.cs`: state machine for ready, playing, game over.
- `ScoreManager.cs`: distance score, bonus score, speed scaling.
- `UIManager.cs`: score, energy, speed, game-over panel, restart.

Deliverable: player can run, dodge, fail and restart.

## Milestone 3: Track Loop

Scripts:

- `TrackSegment.cs`: segment length and spawn/end anchors.
- `TrackSpawner.cs`: active segment queue, spawn ahead, recycle behind.
- `ObjectPool.cs`: optional pool for obstacles, collectibles and particles.

Prefab plan:

- 6 to 8 fixed-length track segment prefabs.
- Each segment uses the same length, recommended `30`.
- Obstacles are manually placed in prefabs to avoid impossible random patterns.

Deliverable: endless forward run with repeating randomized track segments.

## Milestone 4: Energy And Color Gates

Scripts:

- `EnergyModeController.cs`: red, blue, green state, keyboard switching.
- `ColorGate.cs`: pass only if player mode matches gate mode.
- `Collectible.cs`: energy and score pickups.
- `MaterialColorBinder.cs`: update player, track, UI and particles by energy mode.

Deliverable: color switching affects gameplay and visuals.

## Milestone 5: Graphics Showcase

Graphics modules:

- Neon track material: scrolling emission bands.
- Energy gate material: transparent Fresnel and scan lines.
- Dissolve material: threshold plus glowing edge.
- Speed lines: low-risk UI overlay first, URP pass only if time allows.

Scripts:

- `EnergyBurst.cs`: full-energy burst, temporary invulnerability, obstacle clear.
- `DissolveTarget.cs`: animate dissolve threshold.
- `PostProcessController.cs`: adjust Bloom and color grading by speed/burst.
- `EffectToggleManager.cs`: answer-defense demo toggles.

Deliverable: graphics effects can be shown clearly during defense.

## Milestone 6: Polish And Submission

- Replace placeholders with final or free assets.
- Tune speed, jump force, lane distance, collision boxes.
- Add particles and audio feedback.
- Create Windows build.
- Capture screenshots and short gameplay video.
- Complete report and asset source list.

## Suggested Team Split

- A Gameplay: player control, collision, score, UI, failure/restart.
- B Systems: track spawning, object pooling, obstacle/collectible prefab setup.
- C Graphics: shaders/materials, particles, URP post-processing, screenshots/report visuals.

The concrete folder-level split is in `docs/team_tasks.md`.

## Risk Controls

- Build gameplay with primitive objects first.
- Do not wait for final art before testing systems.
- Prefer fixed track segment prefabs over fully procedural generation.
- Implement speed lines as UI overlay first.
- Use Shader Graph unless HLSL is required.
- Commit after every working milestone.
