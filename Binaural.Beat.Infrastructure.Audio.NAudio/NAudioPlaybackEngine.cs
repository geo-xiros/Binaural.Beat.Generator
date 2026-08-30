using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using NAudio.Wave;
using System.Threading;

namespace Binaural.Beat.Infrastructure.Audio.NAudio;

public sealed class NAudioPlaybackEngine : IAudioPlaybackEngine
{
    public void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null, CancellationToken cancellationToken = default)
    {
        var provider = new BinauralBeatProvider(session.LeftFrequency, session.RightFrequency);

        using var waveOut = new WaveOutEvent();
        waveOut.Init(provider);

        try
        {
            waveOut.Play();
            for (int second = 1; second <= durationInSeconds; second++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (cancellationToken.WaitHandle.WaitOne(1000))
                {
                    return;
                }

                onSecondElapsed?.Invoke(second);
            }
        }
        finally
        {
            waveOut.Stop();
        }
    }
}
