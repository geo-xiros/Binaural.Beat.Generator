# Binaural Beat Generator

A simple .NET 10 console app that plays stereo sine waves to produce a **binaural beat** effect through headphones.

This project is inspired by the publication:
- *The Efficiency of Binaural Beats on Anxiety and Depression — A Systematic Review*  
  https://www.researchgate.net/publication/381952306_The_Efficiency_of_Binaural_Beats_on_Anxiety_and_Depression-A_Systematic_Review

## What this program does

The app plays:
- Left channel at a fixed carrier frequency (400 Hz)
- Right channel at `400 Hz + beat frequency`

The perceived binaural beat equals the frequency difference between left and right channels.

Example:
- Left: 400 Hz
- Right: 410 Hz
- Perceived beat: 10 Hz (Alpha range)

## Included brainwave presets

- **Delta (1–4 Hz)** → preset: 2 Hz
- **Theta (4–8 Hz)** → preset: 6 Hz
- **Alpha (8–13 Hz)** → preset: 10 Hz
- **Beta (13–30 Hz)** → preset: 20 Hz
- **Gamma (30–70 Hz)** → preset: 40 Hz

## Console UI behavior

The app uses Spectre.Console for a richer terminal UI:
- Arrow-key menu selection (Up/Down + Enter)
- Duration prompt with validation
- Playback session panel
- Progress bar during playback
- Loop back to menu after each session until **Exit** is selected

## How to use

1. Start the program.
2. Select a target state from the menu.
3. Enter duration in seconds.
4. Wear **stereo headphones** for proper binaural effect.
5. After playback finishes, press a key to return to menu or select **Exit**.

## Build and run

### Prerequisites
- .NET 10 SDK

### Run
```powershell
dotnet restore
dotnet run --project .\Binaural.Beat.Generator\Binaural.Beat.Generator.csproj
```

## Notes and limitations

- This is an educational/demo tool.
- It does not diagnose, treat, or prevent medical or mental health conditions.
- Research findings in this area are mixed and depend on study design, duration, participant population, and protocol.
- Keep playback volume at a safe listening level.

## Dependencies

- [NAudio](https://github.com/naudio/NAudio) — audio output
- [Spectre.Console](https://spectreconsole.net/) — terminal UI
