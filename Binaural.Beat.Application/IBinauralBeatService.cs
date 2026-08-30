using Binaural.Beat.Domain;
using System.Threading;

namespace Binaural.Beat.Application;

public interface IBinauralBeatService
{
    IReadOnlyList<BinauralPreset> GetPresets();
    BinauralSession CreateSession(int Choice);
    void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null, CancellationToken CancellationToken = default);
}
