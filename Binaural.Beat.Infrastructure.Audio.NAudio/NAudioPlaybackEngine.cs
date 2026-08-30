using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using NAudio.Wave;
using System.Threading;

namespace Binaural.Beat.Infrastructure.Audio.NAudio;

public sealed class NAudioPlaybackEngine : IAudioPlaybackEngine
{
    public void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null, CancellationToken CancellationToken = default)
    {
        var Provider = new BinauralBeatProvider(Session.LeftFrequency, Session.RightFrequency);
        using var WaveOut = new WaveOutEvent();
        WaveOut.Init(Provider);

        try
        {
            WaveOut.Play();
            for (int Second = 1; Second <= DurationInSeconds; Second++)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (CancellationToken.WaitHandle.WaitOne(1000))
                {
                    return;
                }

                OnSecondElapsed?.Invoke(Second);
            }
        }
        finally
        {
            WaveOut.Stop();
        }
    }
}
