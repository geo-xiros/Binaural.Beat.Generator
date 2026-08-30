using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using Binaural.Beat.Infrastructure.Audio.NAudio;
using Spectre.Console;


class Program
{
    private record Choice(string Description, BinauralPreset? BinauralPreset = null);

    static void Main()
    {
        BinauralBeatService service = new(new NAudioPlaybackEngine());
        var presetChoices = service.GetPresets().Select(preset => new Choice(preset.Name, preset)).ToList();

        while (true)
        {
            AnsiConsole.Clear();
            ShowHeader();

            var choice = GetChoice(presetChoices);
            if (choice.BinauralPreset is null)
            {
                AnsiConsole.MarkupLine("[green]Goodbye.[/]");
                break;
            }

            BinauralSession session = service.CreateSession(choice.BinauralPreset);
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

    static Choice GetChoice(IReadOnlyList<Choice> presetChoices)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Choice>()
                .Title("[yellow]Choose target state:[/]")
                .AddChoices([new Choice("Exit"), .. presetChoices])
                .UseConverter(choice => choice.Description));
    }

    static int GetDurationInSeconds()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<int>("[yellow]Duration in seconds:[/]")
                .Validate(duration => duration > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Please enter a positive integer.[/]")));
    }

    static void RunPlaybackScreen(BinauralBeatService service, BinauralSession session, int durationInSeconds)
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
