using System;
using NAudio.Wave;

/// <summary>
/// https://www.researchgate.net/publication/381952306_The_Efficiency_of_Binaural_Beats_on_Anxiety_and_Depression-A_Systematic_Review
/// </summary>
class Program
{
    static void Main()
    {
        Console.WriteLine("=== Real-Time Binaural Beat Player ===");

        while (true)
        {
            int choice = GetChoice();
            if (choice == 0)
            {
                Console.WriteLine("Goodbye.");
                break;
            }

            float beatFreq = GetBeatFrequency(choice);
            int duration = GetDurationInSeconds();
            PlayBinauralBeat(beatFreq, duration);
            Console.WriteLine("Finished.");
            Console.WriteLine();
        }
    }

    static int GetChoice()
    {
        Console.Clear();
        while (true)
        {
            Console.WriteLine("Choose target state:");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Delta (1–4 Hz) => Deep Sleep");
            Console.WriteLine("2. Theta (4–8 Hz) => Meditation");
            Console.WriteLine("3. Alpha (8–13 Hz) => Relaxation");
            Console.WriteLine("4. Beta (13–30 Hz) => Focus");
            Console.WriteLine("5. Gamma (30–70 Hz) => Cognitive Enhancement");
            Console.Write("Selection: ");

            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 0 && choice <= 5)
            {
                return choice;
            }

            Console.Clear();
            Console.WriteLine("Invalid choice. Please enter a number between 0 and 5.");
            Console.WriteLine();
        }
    }

    static float GetBeatFrequency(int choice)
    {
        return choice switch
        {
            1 => 2f,
            2 => 6f,
            3 => 10f,
            4 => 20f,
            5 => 40f,
            _ => throw new ArgumentOutOfRangeException(nameof(choice), "Choice must be between 1 and 5.")
        };
    }

    static int GetDurationInSeconds()
    {

        Console.Clear();
        while (true)
        {
            Console.Write("Duration in seconds: ");
            if (int.TryParse(Console.ReadLine(), out int duration) && duration > 0)
            {
                return duration;
            }

            Console.Clear();
            Console.WriteLine("Invalid duration. Please enter a positive integer.");
        }
    }

    static void PlayBinauralBeat(float beatFreq, int durationInSeconds)
    {
        const float LeftFreq = 400f;
        float rightFreq = LeftFreq + beatFreq;

        Console.WriteLine($"Playing {beatFreq} Hz binaural beat...");
        Console.WriteLine($"Left ear: {LeftFreq} Hz");
        Console.WriteLine($"Right ear: {rightFreq} Hz");

        var provider = new BinauralBeatProvider(LeftFreq, rightFreq);
        using var waveOut = new WaveOutEvent();
        waveOut.Init(provider);
        waveOut.Play();

        System.Threading.Thread.Sleep(durationInSeconds * 1000);

        waveOut.Stop();
    }
}
