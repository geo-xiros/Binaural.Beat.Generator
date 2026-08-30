using Binaural.Beat.Domain;

namespace Binaural.Beat.Application;

public sealed class BinauralBeatService(IAudioPlaybackEngine PlaybackEngine) : IBinauralBeatService
{
    private const float LeftCarrierFrequency = 400f;

    private static readonly List<BinauralPreset> Presets =
    [
        new(1, "Delta (1–4 Hz) => Deep Sleep", 2f),
        new(2, "Theta (4–8 Hz) => Meditation", 6f),
        new(3, "Alpha (8–13 Hz) => Relaxation", 10f),
        new(4, "Beta (13–30 Hz) => Focus", 20f),
        new(5, "Gamma (30–70 Hz) => Cognitive Enhancement", 40f)
    ];

    public IReadOnlyList<BinauralPreset> GetPresets()
    {
        return [.. Presets];
    }

    public BinauralSession CreateSession(int Choice)
    {
        BinauralPreset Preset = Presets.Find(Item => Item.Choice == Choice)
            ?? throw new ArgumentOutOfRangeException(nameof(Choice), "Choice must match an available preset.");

        float RightFrequency = LeftCarrierFrequency + Preset.BeatFrequency;
        return new BinauralSession(Preset.Name, Preset.BeatFrequency, LeftCarrierFrequency, RightFrequency);
    }

    public void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null)
    {
        if (DurationInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(DurationInSeconds), "Duration must be greater than zero.");
        }

        PlaybackEngine.Play(Session, DurationInSeconds, OnSecondElapsed);
    }
}
