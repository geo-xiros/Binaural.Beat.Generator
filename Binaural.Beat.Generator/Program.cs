using System;
using Spectre.Console;

/// <summary>
/// https://www.researchgate.net/publication/381952306_The_Efficiency_of_Binaural_Beats_on_Anxiety_and_Depression-A_Systematic_Review
/// </summary>
class Program
{
    private static readonly BinauralBeatService Service = new();

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

            BinauralSession session = Service.CreateSession(choice);
            int duration = GetDurationInSeconds();
            PlayBinauralBeat(session, duration);
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
        var presets = Service.GetPresets();
        var prompt = new SelectionPrompt<int>()
            .Title("[yellow]Choose target state:[/]")
            .PageSize(presets.Count + 1)
            .UseConverter(choice => choice switch
            {
                0 => "Exit",
                _ => presets.Find(preset => preset.Choice == choice)?.Name ?? "Unknown"
            });

        prompt.AddChoices([0, .. presets.ConvertAll(preset => preset.Choice)]);
        return AnsiConsole.Prompt(prompt);
    }

    static int GetDurationInSeconds()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]Duration in seconds:[/]")
                .Validate(duration => duration > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Please enter a positive integer.[/]")));
    }

    static void PlayBinauralBeat(BinauralSession session, int durationInSeconds)
    {
        AnsiConsole.Write(
            new Panel($"[bold]Playing:[/] [green]{session.BeatFrequency} Hz[/]\nLeft ear: [cyan]{session.LeftFrequency} Hz[/]\nRight ear: [cyan]{session.RightFrequency} Hz[/]")
                .Header($"[white]{session.Name} Session[/]")
                .Border(BoxBorder.Rounded)
                .BorderColor(Color.Green));

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
                Service.Play(session, durationInSeconds, _ => task.Increment(1));
            });
    }
}
