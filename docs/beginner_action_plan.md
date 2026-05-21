# Beginner Action Plan

This is the practical plan for teammates who have not used Unity before. Follow it in order. Do not start polishing art before the capsule prototype works.

## Current Progress

Already done in the repository:

- Git repository initialized.
- GitHub remote configured.
- Planning docs added.
- Team task split added.
- First C# scripts added under `Assets/Scripts`.

Not done yet:

- Unity URP project has not been generated.
- `Packages/` and `ProjectSettings/` do not exist yet.
- Main scene, prefabs, materials and UI have not been created in Unity.

## The First Target

Within 48 hours, finish this prototype:

- A capsule player moves forward.
- A/D switches lanes.
- Space jumps.
- S slides.
- A simple track spawns repeatedly.
- Hitting an obstacle causes game over.
- R restarts.
- Score increases.

If this works, the project is viable.

## What You Should Do First

Owner: 你

### Step 1: Install Unity

Install with Unity Hub:

- Unity Editor: `2022.3 LTS`
- Template to use later: `3D (URP)`
- Module: `Windows Build Support`
- IDE: Visual Studio Community if you do not already have a C# IDE

You do not need Android, WebGL, iOS or Linux modules now.

### Step 2: Create The Unity Project

Try creating the project directly in:

```text
D:\programing\code\cghw\endless_runner
```

Use:

- Template: `3D (URP)`
- Project name: `endless_runner` or `NeonRush`

If Unity Hub refuses because the folder is not empty:

1. Create the Unity project in a temporary folder.
2. Close Unity.
3. Move generated folders/files into this repository:
   - `Assets`
   - `Packages`
   - `ProjectSettings`
   - any `.sln` or `.csproj` files can be ignored by Git.
4. Keep existing files:
   - `.git`
   - `.gitignore`
   - `README.md`
   - `docs`
   - `game.txt`

### Step 3: Open Unity And Wait

The first import can take several minutes. Wait until Unity stops compiling.

If console errors appear, screenshot them and send them before changing random settings.

### Step 4: Create The Main Scene

In Unity:

1. Create folder `Assets/Scenes`.
2. Save scene as:

```text
Assets/Scenes/Main.unity
```

Create scene objects:

- `GameManager`
- `TrackSpawner`
- `Player`
- `Main Camera`
- `Directional Light`
- `Global Volume`
- `Canvas`

### Step 5: Make The Capsule Player

1. `GameObject > 3D Object > Capsule`
2. Rename to `Player`
3. Position: `(0, 1, 0)`
4. Add components:
   - `Character Controller`
   - `RunnerController`
   - `PlayerCollision`
   - `EnergyModeController`
   - `EnergyBurst`
5. In `Character Controller`:
   - Height: `2`
   - Radius: `0.4`
   - Center Y: `1`
6. In `EnergyBurst`:
   - Drag `Player` into `Runner`

### Step 6: GameManager Setup

1. Create empty object `GameManager`.
2. Add components:
   - `GameManager`
   - `ScoreManager`
3. In `ScoreManager`:
   - Drag `Player` into `Runner`

### Step 7: TrackSpawner Setup

1. Create empty object `TrackSpawner`.
2. Add component:
   - `TrackSpawner`
3. Drag `Player` into the `Player` field.

For now, it needs one segment prefab from teammate 1.

## Teammate Tasks Based On Current Progress

## 你: Integrator And Gameplay

You own:

```text
Assets/Scenes
Assets/Scripts/Core
Assets/Scripts/Player
Assets/Scripts/UI
```

Today and tomorrow:

- Install Unity.
- Create the URP project.
- Open the project with no compile errors.
- Create `Main.unity`.
- Create capsule player.
- Attach scripts.
- Make sure Play Mode starts.
- Ask me for help when Unity console errors appear.

Do not spend time on:

- Final character model.
- Pretty environment.
- Complex Shader.

Your success condition:

- You press Play and the capsule moves forward.

## 同学 1: Track And Prefabs

They own:

```text
Assets/Prefabs/TrackSegments
Assets/Prefabs/Obstacles
Assets/Prefabs/Collectibles
Assets/Scripts/Track
Assets/Scripts/Gameplay
```

Today and tomorrow:

- Wait until the Unity project exists.
- Create folders:
  - `Assets/Prefabs/TrackSegments`
  - `Assets/Prefabs/Obstacles`
  - `Assets/Prefabs/Collectibles`
- Make one test track segment prefab:
  - root object: `TrackSegment_Test`
  - add `TrackSegment` script to root
  - add Cube child as floor
  - floor position: `(0, -0.05, 15)`
  - floor scale: `(8, 0.1, 30)`
  - save prefab to `Assets/Prefabs/TrackSegments`
- Make one obstacle prefab:
  - Cube
  - add `Obstacle`
  - save to `Assets/Prefabs/Obstacles`
- Make one collectible prefab:
  - Sphere
  - collider set to `Is Trigger`
  - add `Collectible`
  - save to `Assets/Prefabs/Collectibles`

Do not spend time on:

- 6 polished segments immediately.
- Imported models.
- Complicated random generation.

Their success condition:

- The track prefab can be dragged into `TrackSpawner > Segment Prefabs`.

## 同学 2: Visuals And Report

They own:

```text
Assets/Art
Assets/Materials
Assets/Shaders
Assets/VFX
Assets/Prefabs/VFX
docs/asset_sources.md
docs/graphics_notes.md
```

Today and tomorrow:

- Create folders:
  - `Assets/Materials`
  - `Assets/VFX`
  - `Assets/Art`
  - `Assets/Shaders`
- Create placeholder materials:
  - `M_Track_Neon`
  - `M_Obstacle_Neon`
  - `M_Collectible_Energy`
- Use URP/Lit materials first.
- Enable emission colors:
  - track: cyan/blue
  - obstacle: red/magenta
  - collectible: cyan/green
- Help enable Bloom in `Global Volume`.
- Start filling `docs/asset_sources.md` for any external assets.

Do not spend time on:

- Final Shader Graph before the prototype runs.
- Searching assets for hours.
- Replacing the player model.

Their success condition:

- The test scene looks dark + neon instead of default gray.

## Simple Git Rules For The Team

Before working:

```powershell
git pull
```

After a working change:

```powershell
git status
git add .
git commit -m "Short description"
git push
```

Do not commit these:

- `Library`
- `Temp`
- `Obj`
- `Logs`
- `Build`

They are already ignored by `.gitignore`.

## What To Send Me

Send me screenshots or copied error text for:

- Unity Console errors.
- Inspector fields you do not know how to fill.
- Git errors.
- The scene hierarchy if scripts do not work.

The most useful screenshot is:

- Unity Console
- selected object Inspector
- Hierarchy visible on the left

## Do Not Panic About These

It is normal if:

- Unity imports for several minutes.
- `.meta` files appear everywhere.
- Visual Studio opens automatically.
- The first prototype uses only capsules and cubes.
- The scene looks bad for the first few days.

It is not okay if:

- The project has compile errors.
- You cannot enter Play Mode.
- No one owns prefab creation.
- Everyone edits the same scene at the same time.
