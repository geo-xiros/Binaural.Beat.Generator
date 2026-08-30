using NAudio.Wave;

namespace Binaural.Beat.Infrastructure.Audio.NAudio;

internal sealed class BinauralBeatProvider(float LeftFrequency, float RightFrequency) : WaveProvider32(44100, 2)
{
    private float Time;

    public override int Read(float[] Buffer, int Offset, int SampleCount)
    {
        for (int Index = 0; Index < SampleCount; Index += 2)
        {
            float Left = (float)Math.Sin(2 * Math.PI * LeftFrequency * Time);
            float Right = (float)Math.Sin(2 * Math.PI * RightFrequency * Time);

            Buffer[Offset + Index] = Left;
            Buffer[Offset + Index + 1] = Right;

            Time += 1f / WaveFormat.SampleRate;
        }

        return SampleCount;
    }
}
