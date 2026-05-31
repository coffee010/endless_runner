# Graphics Notes

## Player Follow Light

File: `Assets/Scripts/Graphics/PlayerLightFlow.cs`

The player follow light is a small visual effect object that follows the runner from above and behind. It combines a point light, a trail renderer, energy-mode color changes, and a subtle intensity pulse.

Setup:

1. Create an empty GameObject in `Main.unity`, named `PlayerLightFlow`.
2. Add the `PlayerLightFlow` component.
3. Add a child `Point Light`.
4. Add a child or same-object `Trail Renderer`.
5. Leave `Target`, `Runner`, and `Mode Controller` empty if the scene has one `RunnerController`; the script will find it automatically.
6. Make sure Bloom is enabled in the URP volume so the trail and light read as a neon effect.

Recommended values:

- Offset: `(0, 2.2, -1.4)`
- Position Smooth Time: `0.08`
- Base Intensity: `2.2`
- Pulse Intensity: `0.45`
- Light Range: `7`
- Trail Time: `0.35`
- Trail Width: `0.38`
- Emission Multiplier: `3`

Presentation point:

The effect uses `LateUpdate` so the light follows after player movement is resolved. `SmoothDamp` makes the light lag slightly, which creates a more dynamic neon chase feel. The light color is bound to `EnergyModeController`, so switching red or blue energy mode also switches the follow light and trail color.
