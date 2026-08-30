using Binaural.Beat.Domain;
using System.Threading;

namespace Binaural.Beat.Application;

public interface IBinauralBeatService
{
    IReadOnlyList<BinauralPreset> GetPresets();
    BinauralSession CreateSession(BinauralPreset preset);
    void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null, CancellationToken cancellationToken = default);
}
