# Unity Setup Steps

## 0. What To Install

Use Unity Hub to install Unity. Unity Hub is the normal tool for managing Editor versions, modules and projects. Unity's docs describe Hub as the app used to manage Unity Editor installations, modules and projects.

Recommended setup:

- Unity Hub: latest stable Hub.
- Unity Editor: Unity 6 `6000.4.7f1`.
- Template: `Universal 3D` or `3D (URP)`.
- Required module:
  - `Windows Build Support (IL2CPP)` or `Windows Build Support (Mono)`.
- Recommended module:
  - `Microsoft Visual Studio Community` if you do not already have Visual Studio or Rider.
- Optional, not needed now:
  - Android Build Support.
  - WebGL Build Support.
  - iOS/macOS/Linux modules.
  - VFX Graph samples.

Install only Windows build support for now. Extra platform modules can be added later from Unity Hub through `Installs > Manage > Add modules`.

This project is already created with Unity 6 `6000.4.7f1`. Teammates should use the same Editor version when possible. Do not open this project with `2022.3 LTS`, because downgrading a Unity project can create compatibility issues.

## 1. Create The Project

1. Open Unity Hub.
2. Click `New project`.
3. Choose `Universal 3D` or `3D (URP)`.
4. Project name: `endless_runner` or `NeonRush`.
5. Location: choose this repository folder: `D:\programing\code\cghw\endless_runner`.
6. Create the project.

If Unity Hub refuses to create into a non-empty folder, create the project in a temporary folder, close Unity, then move all generated Unity project files into this repository folder. Keep `.git/`, `.gitignore`, `README.md`, `docs/`, and `game.txt`.

After the move, the repository root should contain at least:

```text
Assets/
Packages/
ProjectSettings/
.git/
.gitignore
README.md
docs/
game.txt
```

## 2. Create Folders Under Assets

Create:

```text
Assets/Art
Assets/Materials
Assets/Prefabs
Assets/Scenes
Assets/Scripts
Assets/Shaders
Assets/VFX
Assets/Audio
Assets/ThirdParty
```

Then create the subfolders shown in `README.md`.

Some script folders already exist in this repository:

```text
Assets/Scripts/Core
Assets/Scripts/Player
Assets/Scripts/Track
Assets/Scripts/Gameplay
Assets/Scripts/Graphics
Assets/Scripts/UI
```

## 3. Project Settings

- `Edit > Project Settings > Player`
  - Company Name: your team or school name
  - Product Name: `Neon Rush`
  - Default Screen Width: `1920`
  - Default Screen Height: `1080`
- `Edit > Project Settings > Quality`
  - Use URP quality level.
- `Edit > Project Settings > Graphics`
  - Confirm URP asset is assigned.
- `Edit > Project Settings > Player > Other Settings`
  - Active Input Handling: `Input System Package (New)`.

## 4. Main Scene

1. Save the scene as `Assets/Scenes/Main.unity`.
2. Add empty object `GameManager`.
3. Add empty object `TrackSpawner`.
4. Add Capsule or imported model named `Player`.
5. Add Main Camera behind and above the player.
6. Add `Global Volume`.
7. Add Canvas for score, energy and game-over UI.

## 4.1 Player Setup

Create a temporary player with Unity primitives first:

1. `GameObject > 3D Object > Capsule`.
2. Rename it to `Player`.
3. Position: `(0, 1, 0)`.
4. Add component:
   - `Character Controller`
   - `RunnerController`
   - `PlayerCollision`
   - `EnergyModeController`
   - `EnergyBurst`
5. In `Character Controller`:
   - Height: `2`
   - Radius: `0.4`
   - Center Y: `1`
6. In `EnergyBurst`, drag the `Player` object into its `Runner` field.

Do not import a final character model yet. First make the capsule version playable.

## 4.2 Game Manager Setup

1. Create empty object `GameManager`.
2. Add:
   - `GameManager`
   - `ScoreManager`
3. In `ScoreManager`, drag `Player` into the `Runner` field.

## 4.3 Track Spawner Setup

1. Create empty object `TrackSpawner`.
2. Add component `TrackSpawner`.
3. Drag `Player` into the `Player` field.
4. Leave `Segment Prefabs` empty until teammate 1 creates prefabs.

For the first local test without prefabs, create one simple track segment:

1. Create empty object `TrackSegment_Test`.
2. Add a Cube child:
   - Position: `(0, -0.05, 15)`
   - Scale: `(8, 0.1, 30)`
3. Add `TrackSegment` to the root.
4. Drag root object into `Assets/Prefabs/TrackSegments` to make a prefab.
5. Delete the scene instance.
6. Add the prefab to `TrackSpawner > Segment Prefabs`.

## 4.4 Obstacle And Collectible Setup

Low obstacle:

1. Create Cube.
2. Rename `Obstacle_Low`.
3. Scale: `(1.5, 0.8, 1)`.
4. Add `Obstacle`.
5. Set `Obstacle Type` to `Low`.
6. Make prefab in `Assets/Prefabs/Obstacles`.

High obstacle:

1. Create Cube.
2. Rename `Obstacle_High`.
3. Position Y: `1.5`.
4. Scale: `(1.5, 1, 1)`.
5. Add `Obstacle`.
6. Set `Obstacle Type` to `High`.
7. Make prefab in `Assets/Prefabs/Obstacles`.

Energy collectible:

1. Create Sphere.
2. Rename `Collectible_Energy`.
3. Scale: `(0.5, 0.5, 0.5)`.
4. Add Sphere Collider.
5. Enable `Is Trigger`.
6. Add `Collectible`.
7. Set `Type` to `Energy`.
8. Make prefab in `Assets/Prefabs/Collectibles`.

Put obstacle/collectible prefabs inside track segment prefabs at lane X positions:

- Left: `-2.5`
- Center: `0`
- Right: `2.5`

## 4.5 UI Setup

1. Create `Canvas`.
2. Add Text object named `ScoreText`.
3. Add Text object named `SpeedText`.
4. Add Slider named `EnergySlider`.
5. Add Panel named `GameOverPanel`, with text `Game Over - Press R`.
6. Disable `GameOverPanel` by default.
7. Create empty child or object named `UIManager`.
8. Add `UIManager`.
9. Assign:
   - `ScoreManager`: scene `GameManager` object.
   - `EnergyBurst`: scene `Player` object.
   - `Score Text`: `ScoreText`.
   - `Speed Text`: `SpeedText`.
   - `Energy Slider`: `EnergySlider`.
   - `Game Over Panel`: `GameOverPanel`.

## 5. URP And Post Processing

1. Select Main Camera.
2. Enable post processing on the camera.
3. Create a Global Volume profile.
4. Add:
   - Bloom
   - Color Adjustments
   - Vignette
5. Start with:
   - Bloom Intensity: `1.5`
   - Bloom Threshold: `1.0`
   - Saturation: `15`
   - Contrast: `20`

## 5.1 Materials

Create placeholder materials:

- `Assets/Materials/M_Track_Neon.mat`
- `Assets/Materials/M_Obstacle_Neon.mat`
- `Assets/Materials/M_Collectible_Energy.mat`

Use URP/Lit first:

- Track base color: dark gray.
- Track emission: cyan or blue.
- Obstacle emission: magenta/red.
- Collectible emission: cyan.

Bloom only works visibly when materials use emission colors above normal brightness.

## 6. First Git Commit

After Unity creates the project and the scene opens without errors:

```powershell
git status
git add .
git commit -m "Bootstrap Unity URP project"
```

Then push:

```powershell
git push
```

## 7. Link Remote Repository

The remote is already configured:

```powershell
git@github.com:coffee010/endless_runner.git
```

Use normal workflow after this:

```powershell
git pull
git status
git add .
git commit -m "Describe your change"
git push
```

## 8. How To Work After Unity Is Created

Daily workflow:

1. `git pull`.
2. Open Unity Hub.
3. Open this project folder.
4. Work only in your assigned folders from `docs/team_tasks.md`.
5. Test Play Mode.
6. Save scene/prefabs.
7. Close Unity or wait for Unity to finish importing.
8. `git status`.
9. Commit focused changes.
10. `git push`.

Do not commit:

- `Library/`
- `Temp/`
- `Obj/`
- `Logs/`
- `Build/`

The `.gitignore` already excludes these.

## 9. First Playable Test Checklist

Before adding final art, confirm:

- Player capsule moves forward automatically.
- A/D switches lanes.
- Space jumps.
- S slides.
- Track segments spawn ahead.
- Player colliding with an obstacle causes game over.
- R restarts.
- Score text increases.
- Energy collectible fills the energy slider.
- F triggers energy burst after energy is full.
