using System;

interface IAudioPlaybackEngine
{
    void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null);
}
