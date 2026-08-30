using NAudio.Wave;

class BinauralBeatService
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

    public List<BinauralPreset> GetPresets()
    {
        return [.. Presets];
    }

    public BinauralSession CreateSession(int choice)
    {
        BinauralPreset? preset = Presets.Find(item => item.Choice == choice);
        if (preset is null)
        {
            throw new ArgumentOutOfRangeException(nameof(choice), "Choice must match an available preset.");
        }

        float rightFrequency = LeftCarrierFrequency + preset.BeatFrequency;
        return new BinauralSession(preset.Name, preset.BeatFrequency, LeftCarrierFrequency, rightFrequency);
    }

    public void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null)
    {
        var provider = new BinauralBeatProvider(session.LeftFrequency, session.RightFrequency);
        using var waveOut = new WaveOutEvent();
        waveOut.Init(provider);

        try
        {
            waveOut.Play();
            for (int second = 1; second <= durationInSeconds; second++)
            {
                Thread.Sleep(1000);
                onSecondElapsed?.Invoke(second);
            }
        }
        finally
        {
            waveOut.Stop();
        }
    }
}

record BinauralPreset(int Choice, string Name, float BeatFrequency);
record BinauralSession(string Name, float BeatFrequency, float LeftFrequency, float RightFrequency);
