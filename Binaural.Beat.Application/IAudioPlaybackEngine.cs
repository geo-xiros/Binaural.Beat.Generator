using Binaural.Beat.Domain;
using System.Threading;

namespace Binaural.Beat.Application;

public interface IAudioPlaybackEngine
{
    void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null, CancellationToken cancellationToken = default);
}
