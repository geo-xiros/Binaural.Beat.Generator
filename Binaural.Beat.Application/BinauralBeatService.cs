using Binaural.Beat.Domain;

namespace Binaural.Beat.Application;

public sealed class BinauralBeatService(IAudioPlaybackEngine playbackEngine) : IBinauralBeatService
{
    private const float leftCarrierFrequency = 400f;

    private static readonly List<BinauralPreset> presets =
    [
        new("Delta (1–4 Hz) => Deep Sleep", 2f),
        new("Theta (4–8 Hz) => Meditation", 6f),
        new("Alpha (8–13 Hz) => Relaxation", 10f),
        new("Beta (13–30 Hz) => Focus", 20f),
        new("Gamma (30–70 Hz) => Cognitive Enhancement", 40f)
    ];

    public IReadOnlyList<BinauralPreset> GetPresets()
    {
        return [.. presets];
    }

    public BinauralSession CreateSession(BinauralPreset preset)
    {
        float rightFrequency = leftCarrierFrequency + preset.BeatFrequency;
        return new BinauralSession(preset.Name, preset.BeatFrequency, leftCarrierFrequency, rightFrequency);
    }

    public void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null, CancellationToken cancellationToken = default)
    {
        if (durationInSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(durationInSeconds), "Duration must be greater than zero.");
        }

        playbackEngine.Play(session, durationInSeconds, onSecondElapsed, cancellationToken);
    }
}
