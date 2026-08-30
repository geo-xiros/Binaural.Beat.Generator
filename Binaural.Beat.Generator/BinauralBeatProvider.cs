using NAudio.Wave;

class BinauralBeatProvider(float leftFreq, float rightFreq) : WaveProvider32(44100, 2)
{
    private float t;

    public override int Read(float[] buffer, int offset, int sampleCount)
    {
        for (int n = 0; n < sampleCount; n += 2)
        {
            float left = (float)Math.Sin(2 * Math.PI * leftFreq * t);
            float right = (float)Math.Sin(2 * Math.PI * rightFreq * t);

            buffer[offset + n] = left;     // Left channel
            buffer[offset + n + 1] = right; // Right channel

            t += 1f / WaveFormat.SampleRate;
        }

        return sampleCount;
    }
}
