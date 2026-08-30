using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Binaural.Beat.WpfClient;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IBinauralBeatService service;
    private CancellationTokenSource? playbackCancellationTokenSource;

    private IReadOnlyList<BinauralPreset> presets = [];
    private BinauralPreset? selectedPreset;
    private string durationText = "30";
    private string sessionNameText = "Session: -";
    private string leftFrequencyText = "Left ear: -";
    private string rightFrequencyText = "Right ear: -";
    private string status = "Ready";
    private int progressMaximum = 100;
    private int progressValue;
    private bool isPlaying;

    public MainWindowViewModel(IBinauralBeatService service)
    {
        this.service = service;

        PlayCommand = new RelayCommand(async () => await PlayAsync(), () => !IsPlaying);
        StopCommand = new RelayCommand(Stop, () => IsPlaying);

        LoadPresets();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<BinauralPreset> Presets
    {
        get => presets;
        private set => SetProperty(ref presets, value);
    }

    public BinauralPreset? SelectedPreset
    {
        get => selectedPreset;
        set => SetProperty(ref selectedPreset, value);
    }

    public string DurationText
    {
        get => durationText;
        set => SetProperty(ref durationText, value);
    }

    public string SessionNameText
    {
        get => sessionNameText;
        private set => SetProperty(ref sessionNameText, value);
    }

    public string LeftFrequencyText
    {
        get => leftFrequencyText;
        private set => SetProperty(ref leftFrequencyText, value);
    }

    public string RightFrequencyText
    {
        get => rightFrequencyText;
        private set => SetProperty(ref rightFrequencyText, value);
    }

    public string Status
    {
        get => status;
        private set => SetProperty(ref status, value);
    }

    public int ProgressMaximum
    {
        get => progressMaximum;
        private set => SetProperty(ref progressMaximum, value);
    }

    public int ProgressValue
    {
        get => progressValue;
        private set => SetProperty(ref progressValue, value);
    }

    public bool IsPlaying
    {
        get => isPlaying;
        private set
        {
            if (!SetProperty(ref isPlaying, value))
            {
                return;
            }

            OnPropertyChanged(nameof(CanEditInputs));
            RaiseCommandStates();
        }
    }

    public bool CanEditInputs => !IsPlaying;

    public ICommand PlayCommand { get; }

    public ICommand StopCommand { get; }

    private void LoadPresets()
    {
        Presets = service.GetPresets();
        SelectedPreset = Presets.FirstOrDefault();
    }

    private async Task PlayAsync()
    {
        if (SelectedPreset is null)
        {
            MessageBox.Show("Please select a target state.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(DurationText, out int durationInSeconds) || durationInSeconds <= 0)
        {
            MessageBox.Show("Please enter a positive duration in seconds.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        BinauralSession session = service.CreateSession(SelectedPreset.Choice);
        BindSession(session);

        ProgressMaximum = durationInSeconds;
        ProgressValue = 0;
        Status = "Playing...";

        try
        {
            IsPlaying = true;
            playbackCancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                service.Play(session, durationInSeconds, second =>
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProgressValue = second;
                        Status = $"Playing... {second}/{durationInSeconds}s";
                    });
                }, playbackCancellationTokenSource.Token);
            });

            Status = "Finished.";
        }
        catch (OperationCanceledException)
        {
            Status = "Stopped.";
        }
        catch (Exception ex)
        {
            Status = "Playback failed.";
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            playbackCancellationTokenSource?.Dispose();
            playbackCancellationTokenSource = null;
            IsPlaying = false;
        }
    }

    private void Stop()
    {
        playbackCancellationTokenSource?.Cancel();
    }

    private void BindSession(BinauralSession session)
    {
        SessionNameText = $"Session: {session.Name}";
        LeftFrequencyText = $"Left ear: {session.LeftFrequency:F1} Hz";
        RightFrequencyText = $"Right ear: {session.RightFrequency:F1} Hz";
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RaiseCommandStates()
    {
        if (PlayCommand is RelayCommand play)
        {
            play.RaiseCanExecuteChanged();
        }

        if (StopCommand is RelayCommand stop)
        {
            stop.RaiseCanExecuteChanged();
        }
    }
}
