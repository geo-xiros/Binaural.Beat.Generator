using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using Binaural.Beat.Infrastructure.Audio.NAudio;
using Spectre.Console;


class Program
{
    static void Main()
    {
        IBinauralBeatService service = new BinauralBeatService(new NAudioPlaybackEngine());

        while (true)
        {
            AnsiConsole.Clear();
            ShowHeader();

            int choice = GetChoice(service.GetPresets());
            if (choice == 0)
            {
                AnsiConsole.MarkupLine("[green]Goodbye.[/]");
                break;
            }

            BinauralSession session = service.CreateSession(choice);
            int duration = GetDurationInSeconds();

            RunPlaybackScreen(service, session, duration);
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

    static int GetChoice(IReadOnlyList<BinauralPreset> binauralPresets)
    {
        var prompt = new SelectionPrompt<int>()
            .Title("[yellow]Choose target state:[/]")
            .PageSize(binauralPresets.Count + 1)
            .UseConverter(choice => choice switch
            {
                0 => "Exit",
                _ => binauralPresets.FirstOrDefault(preset => preset.Choice == choice)?.Name ?? "Unknown"
            });

        prompt.AddChoices([0, .. binauralPresets.Select(preset => preset.Choice)]);
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

    static void RunPlaybackScreen(IBinauralBeatService service, BinauralSession session, int durationInSeconds)
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
                service.Play(session, durationInSeconds, _ => task.Increment(1));
            });
    }
}
