using NAudio.Wave;

class NAudioPlaybackEngine : IAudioPlaybackEngine
{
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
