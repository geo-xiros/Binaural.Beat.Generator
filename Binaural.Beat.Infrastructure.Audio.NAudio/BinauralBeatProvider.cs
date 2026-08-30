using NAudio.Wave;

namespace Binaural.Beat.Infrastructure.Audio.NAudio;

internal sealed class BinauralBeatProvider(float leftFrequency, float rightFrequency) : WaveProvider32(44100, 2)
{
    private float Time;

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        for (int index = 0; index < sampleCount; index += 2)
        {
            float left = (float)Math.Sin(2 * Math.PI * leftFrequency * Time);
            float right = (float)Math.Sin(2 * Math.PI * rightFrequency * Time);

            buffer[offset + index] = left;
            buffer[offset + index + 1] = right;

            Time += 1f / WaveFormat.SampleRate;
        }

        return sampleCount;
    }
}
