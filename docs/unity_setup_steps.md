# Unity Setup Steps

## 1. Create The Project

1. Open Unity Hub.
2. Click `New project`.
3. Choose `3D (URP)`.
4. Project name: `endless_runner` or `NeonRush`.
5. Location: choose this repository folder: `D:\programing\code\cghw\endless_runner`.
6. Create the project.

If Unity Hub refuses to create into a non-empty folder, create the project in a temporary folder, close Unity, then move all generated Unity project files into this repository folder. Keep `.git/`, `.gitignore`, `README.md`, `docs/`, and `game.txt`.

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
- `Edit > Project Settings > Input Manager`
  - Keep default axes for now.

## 4. Main Scene

1. Save the scene as `Assets/Scenes/Main.unity`.
2. Add empty object `GameManager`.
3. Add empty object `TrackSpawner`.
4. Add Capsule or imported model named `Player`.
5. Add Main Camera behind and above the player.
6. Add `Global Volume`.
7. Add Canvas for score, energy and game-over UI.

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

## 6. First Git Commit

After Unity creates the project and the scene opens without errors:

```powershell
git status
git add .
git commit -m "Bootstrap Unity URP project"
```

## 7. Link Remote Repository

If GitHub already gave you a URL, run:

```powershell
git remote add origin YOUR_GITHUB_REPOSITORY_URL
git push -u origin main
```

Replace `YOUR_GITHUB_REPOSITORY_URL` with your actual HTTPS or SSH URL.
