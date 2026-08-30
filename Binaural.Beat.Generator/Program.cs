using System;
using System.Threading;
using NAudio.Wave;
using Spectre.Console;

/// <summary>
/// https://www.researchgate.net/publication/381952306_The_Efficiency_of_Binaural_Beats_on_Anxiety_and_Depression-A_Systematic_Review
/// </summary>
class Program
{
    static void Main()
    {
        while (true)
        {
            AnsiConsole.Clear();
            ShowHeader();

            int choice = GetChoice();
            if (choice == 0)
            {
                AnsiConsole.MarkupLine("[green]Goodbye.[/]");
                break;
            }

            float beatFreq = GetBeatFrequency(choice);
            int duration = GetDurationInSeconds();
            PlayBinauralBeat(beatFreq, duration);
            AnsiConsole.MarkupLine("[green]Finished.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to return to the menu...[/]");
            Console.ReadKey(true);
        }
    }

    static void ShowHeader()
    {
        AnsiConsole.Write(
            new Panel("[bold cyan]Real-Time Binaural Beat Player[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Cyan1));
        AnsiConsole.WriteLine();
    }

    static int GetChoice()
    {
        var prompt = new SelectionPrompt<int>()
            .Title("[yellow]Choose target state:[/]")
            .PageSize(6)
            .UseConverter(choice => choice switch
            {
                0 => "Exit",
                1 => "Delta (1–4 Hz) => Deep Sleep",
                2 => "Theta (4–8 Hz) => Meditation",
                3 => "Alpha (8–13 Hz) => Relaxation",
                4 => "Beta (13–30 Hz) => Focus",
                5 => "Gamma (30–70 Hz) => Cognitive Enhancement",
                _ => "Unknown"
            });

        prompt.AddChoices([0, 1, 2, 3, 4, 5]);
        return AnsiConsole.Prompt(prompt);
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
        return AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]Duration in seconds:[/]")
                .Validate(duration => duration > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Please enter a positive integer.[/]")));
    }

    static void PlayBinauralBeat(float beatFreq, int durationInSeconds)
    {
        const float LeftFreq = 400f;
        float rightFreq = LeftFreq + beatFreq;

        AnsiConsole.Write(
            new Panel($"[bold]Playing:[/] [green]{beatFreq} Hz[/]\nLeft ear: [cyan]{LeftFreq} Hz[/]\nRight ear: [cyan]{rightFreq} Hz[/]")
                .Header("[white]Session[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Green));

        var provider = new BinauralBeatProvider(LeftFreq, rightFreq);
        using var waveOut = new WaveOutEvent();
        waveOut.Init(provider);

        try
        {
            waveOut.Play();

            AnsiConsole.Progress()
                .Columns(
                [
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn()
                ])
                .Start(ctx =>
                {
                    var task = ctx.AddTask("[green]Playing[/]", maxValue: durationInSeconds);
                    while (!task.IsFinished)
                    {
                        Thread.Sleep(1000);
                        task.Increment(1);
                    }
                });
        }
        finally
        {
            waveOut.Stop();
        }
    }
}
