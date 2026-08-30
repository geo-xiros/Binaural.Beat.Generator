using Binaural.Beat.Domain;
using System.Threading;

namespace Binaural.Beat.Application;

public sealed class BinauralBeatService(IAudioPlaybackEngine playbackEngine) : IBinauralBeatService
{
    private const float leftCarrierFrequency = 400f;

    private static readonly List<BinauralPreset> presets =
    [
        new(1, "Delta (1–4 Hz) => Deep Sleep", 2f),
        new(2, "Theta (4–8 Hz) => Meditation", 6f),
        new(3, "Alpha (8–13 Hz) => Relaxation", 10f),
        new(4, "Beta (13–30 Hz) => Focus", 20f),
        new(5, "Gamma (30–70 Hz) => Cognitive Enhancement", 40f)
    ];

    public IReadOnlyList<BinauralPreset> GetPresets()
    {
        return [.. presets];
    }

    public BinauralSession CreateSession(int Choice)
    {
        BinauralPreset Preset = presets.Find(Item => Item.Choice == Choice)
            ?? throw new ArgumentOutOfRangeException(nameof(Choice), "Choice must match an available preset.");

        float RightFrequency = leftCarrierFrequency + Preset.BeatFrequency;
        return new BinauralSession(Preset.Name, Preset.BeatFrequency, leftCarrierFrequency, RightFrequency);
    }

    public void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null, CancellationToken CancellationToken = default)
    {
        if (DurationInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(DurationInSeconds), "Duration must be greater than zero.");
        }

        playbackEngine.Play(Session, DurationInSeconds, OnSecondElapsed, CancellationToken);
    }
}
