using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using NAudio.Wave;

namespace Binaural.Beat.Infrastructure.Audio.NAudio;

public sealed class NAudioPlaybackEngine : IAudioPlaybackEngine
{
    public void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null)
    {
        var Provider = new BinauralBeatProvider(Session.LeftFrequency, Session.RightFrequency);
        using var WaveOut = new WaveOutEvent();
        WaveOut.Init(Provider);

        try
        {
            WaveOut.Play();
            for (int Second = 1; Second <= DurationInSeconds; Second++)
            {
                Thread.Sleep(1000);
                OnSecondElapsed?.Invoke(Second);
            }
        }
        finally
        {
            WaveOut.Stop();
        }
    }
}
