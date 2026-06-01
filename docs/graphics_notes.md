# Graphics Notes

This document summarizes the visual work for **Neon Rush**. It can be used as source material for the project report and presentation.

## Visual Direction

The game uses a cyberpunk neon style: dark road surfaces, bright cyan/magenta light strips, strong bloom, colored energy gates, and short particle bursts for gameplay feedback. The goal is to make the track readable while still giving the scene a fast, futuristic look.

## Materials

Main material groups are stored in `Assets/Materials`.

| Material | Purpose |
| --- | --- |
| `M_CyberRoad_Dark` / `M_CyberRoad_Readable` | Dark road and platform surfaces. |
| `M_CyberLane_Cyan` | Cyan lane lines and moving light strips. |
| `M_CyberEdge_Magenta` | Magenta runway edge highlights. |
| `M_CyberGate_Red` / `M_CyberGate_Blue` | Red and blue energy gate colors. |
| `M_Obstacle_Neon` | Bright warning color for obstacles. |
| `M_Collectible_Energy` | Emissive collectible material. |
| `M_CyberKit_*` | Retinted cyberpunk environment materials. |

Most gameplay materials use emission so they react strongly to Bloom. Interactive objects are brighter than background props, helping the player quickly identify gates, obstacles, and collectibles.

## Shader Effects

Shader files are stored in `Assets/Shades`.

### Cyber Track Shader

File: `Assets/Shades/S_CyberTrack.shader`

This shader creates a dark road surface with animated neon details. It uses UV coordinates and `_Time` to move glowing grid and edge patterns backward along the track. Important parameters:

- `_BaseColor`: dark road base color.
- `_EmissionColor`: main neon color.
- `_GridDensity`: density of repeated grid lines.
- `_FlowSpeed`: speed of the animated motion.
- `_EmissionPower`: brightness of the neon effect.
- `_EdgeIntensity`: brightness of the edge flow.
- `_EdgePulseDensity`: number of moving light pulses.
- `_EdgePulseLength`: length of each pulse.

The animated edge pulses make the road feel like it is moving quickly under the player.

### Flowing Neon Strip Shader

File: `Assets/Shades/S_FlowingNeonStrip.shader`

This shader is used for narrow runway edge and lane light strips. It generates repeated moving pulses along the strip, adds a slight breathing brightness, and applies a rim glow based on view angle.

It is used by:

- `M_CyberLane_Cyan`
- `M_CyberEdge_Magenta`

Important parameters:

- `_FlowSpeed`: direction and speed of the moving pulse.
- `_PulseDensity`: how many pulses appear along the strip.
- `_PulseLength`: how long each pulse is.
- `_CoreWidth`: width of the bright center line.
- `_RimPower`: edge highlight strength.

### Energy Gate Shader

File: `Assets/Shades/S_EnergyGate.shader`

This shader supports the glowing energy-gate look. Red and blue gate colors are driven by the gate mode, making it clear which energy mode the player needs.

### Speed Lines Overlay

File: `Assets/Shades/S_SpeedLinesOverlay.shader`

This overlay adds a lightweight speed-line effect near the camera. It supports the feeling of forward motion without adding extra 3D objects to the track.

## Particle Effects

Particle prefabs are stored in `Assets/Prefabs/VFX`.

| VFX Prefab | Purpose |
| --- | --- |
| `VFX_Collect.prefab` | Spawned when the player collects an item. |
| `VFX_Hit.prefab` | Spawned when the player hits an obstacle or fails a gate. |
| `VFX_GatePass.prefab` | Spawned when the player passes a color gate. |
| `VFX_Burst.prefab` | Spawned when the player triggers the energy burst. |

Runtime VFX spawning is handled through `Assets/Scripts/Gameplay/VfxUtility.cs`. The utility makes particle effects use unscaled time, so collision VFX can still play when the game enters Game Over and `Time.timeScale` becomes `0`.

Color gate VFX are tinted at runtime:

- Blue gate feedback uses blue particles.
- Red gate feedback uses red particles.

The VFX scale can be adjusted in the Inspector:

- `TrackSpawner > Obstacle Collision Vfx Scale`
- `TrackSpawner > Gate Vfx Scale`
- `Player > EnergyBurst > Burst Vfx Scale`

## Player Follow Glow

File: `Assets/Scripts/Player/PlayerFollowGlow.cs`

The player has a follow glow effect made from runtime-created particle systems and a point light. It stays behind the player and creates a short trail while running. The color follows the player's current energy mode:

- Blue mode: cyan-blue glow.
- Red mode: red-pink glow.

The effect uses world-space particles so the glow remains behind the player as the player moves forward.

## Post-Processing

URP settings and volume profiles are stored in `Assets/Settings`.

The main post-processing choices are:

- **Bloom**: makes emissive materials and particles glow.
- **Color Adjustments**: pushes the scene toward a cool cyberpunk look.
- **Vignette**: subtly darkens screen edges and keeps focus near the player.
- **Fog**: adds depth and separates foreground gameplay objects from distant scenery.

Bloom is important because many materials use emission. Without Bloom, the neon surfaces still have color, but they lose the bright cyberpunk glow.

## Readability Rules

The visual design follows these rules:

- Gameplay objects should be brighter than background props.
- Obstacles should use warning colors and clear silhouettes.
- Gates should clearly show red or blue mode.
- Road edge lights should show speed but not overpower the player.
- Particle effects should be large enough to read but short enough not to block gameplay.

## Current Tuning Notes

- Road edge flow was reduced after testing because the first version was too bright.
- Collision and gate VFX were enlarged because the original particle size was hard to see during fast movement.
- Gate VFX are now color-tinted at runtime, so blue and red gates have different feedback.
- Hit and burst particles use unscaled time so they remain visible during Game Over or pause-like states.

## Suggested Screenshots For Report

Capture these images for the final report or presentation:

- Full gameplay view showing the neon road.
- Close-up of flowing lane or runway-edge lights.
- Red gate and blue gate comparison.
- Obstacle hit VFX.
- Gate pass VFX.
- Energy burst VFX.
- Player follow glow in blue mode and red mode.
- Bloom-enabled scene view.

## Files To Mention In Presentation

- `Assets/Shades/S_CyberTrack.shader`
- `Assets/Shades/S_FlowingNeonStrip.shader`
- `Assets/Shades/S_EnergyGate.shader`
- `Assets/Shades/S_SpeedLinesOverlay.shader`
- `Assets/Scripts/Gameplay/VfxUtility.cs`
- `Assets/Scripts/Player/PlayerFollowGlow.cs`
- `Assets/Prefabs/VFX`
- `Assets/Materials`
- `docs/asset_sources.md`
