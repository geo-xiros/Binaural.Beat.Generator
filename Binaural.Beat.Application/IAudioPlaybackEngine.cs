using Binaural.Beat.Domain;

namespace Binaural.Beat.Application;

public interface IAudioPlaybackEngine
{
    void Play(BinauralSession Session, int DurationInSeconds, Action<int>? OnSecondElapsed = null);
}
