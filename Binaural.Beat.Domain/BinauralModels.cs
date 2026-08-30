namespace Binaural.Beat.Domain;

public sealed record BinauralPreset(string Name, float BeatFrequency);

public sealed record BinauralSession(string Name, float BeatFrequency, float LeftFrequency, float RightFrequency);
