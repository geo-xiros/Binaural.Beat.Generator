namespace Binaural.Beat.Domain;

public sealed record BinauralPreset(int Choice, string Name, float BeatFrequency);
public sealed record BinauralSession(string Name, float BeatFrequency, float LeftFrequency, float RightFrequency);
