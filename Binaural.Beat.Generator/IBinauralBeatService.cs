using System;
using System.Collections.Generic;

interface IBinauralBeatService
{
    IReadOnlyList<BinauralPreset> GetPresets();
    BinauralSession CreateSession(int choice);
    void Play(BinauralSession session, int durationInSeconds, Action<int>? onSecondElapsed = null);
}
