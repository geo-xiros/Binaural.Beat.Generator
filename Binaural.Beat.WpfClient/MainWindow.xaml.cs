using Binaural.Beat.Application;
using Binaural.Beat.Domain;
using Binaural.Beat.Infrastructure.Audio.NAudio;
using MahApps.Metro.Controls;
using System.Windows;

namespace Binaural.Beat.WpfClient;

public partial class MainWindow : MetroWindow
{
    private readonly IBinauralBeatService service = new BinauralBeatService(new NAudioPlaybackEngine());
    private IReadOnlyList<BinauralPreset> presets = [];
    private CancellationTokenSource? playbackCancellationTokenSource;

    public MainWindow()
    {
        InitializeComponent();
        LoadPresets();
    }

    private void LoadPresets()
    {
        presets = service.GetPresets();
        PresetComboBox.ItemsSource = presets;
        PresetComboBox.SelectedIndex = 0;
    }

    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        if (PresetComboBox.SelectedItem is not BinauralPreset selectedPreset)
        {
            MessageBox.Show("Please select a target state.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(DurationTextBox.Text, out int durationInSeconds) || durationInSeconds <= 0)
        {
            MessageBox.Show("Please enter a positive duration in seconds.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            DurationTextBox.Focus();
            DurationTextBox.SelectAll();
            return;
        }

        BinauralSession session = service.CreateSession(selectedPreset.Choice);
        BindSession(session);

        PlaybackProgressBar.Minimum = 0;
        PlaybackProgressBar.Maximum = durationInSeconds;
        PlaybackProgressBar.Value = 0;

        SetControlsEnabled(false);
        StopButton.IsEnabled = true;
        StatusTextBlock.Text = "Playing...";

        try
        {
            playbackCancellationTokenSource = new CancellationTokenSource();

            await Task.Run(() =>
            {
                service.Play(session, durationInSeconds, second =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        PlaybackProgressBar.Value = second;
                        StatusTextBlock.Text = $"Playing... {second}/{durationInSeconds}s";
                    });
                }, playbackCancellationTokenSource.Token);
            });

            StatusTextBlock.Text = "Finished.";
        }
        catch (OperationCanceledException)
        {
            StatusTextBlock.Text = "Stopped.";
        }
        catch (Exception ex)
        {
            StatusTextBlock.Text = "Playback failed.";
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            playbackCancellationTokenSource?.Dispose();
            playbackCancellationTokenSource = null;
            SetControlsEnabled(true);
            StopButton.IsEnabled = false;
        }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        playbackCancellationTokenSource?.Cancel();
    }

    private void BindSession(BinauralSession session)
    {
        SessionNameTextBlock.Text = $"Session: {session.Name}";
        LeftFrequencyTextBlock.Text = $"Left ear: {session.LeftFrequency:F1} Hz";
        RightFrequencyTextBlock.Text = $"Right ear: {session.RightFrequency:F1} Hz";
    }

    private void SetControlsEnabled(bool isEnabled)
    {
        PresetComboBox.IsEnabled = isEnabled;
        DurationTextBox.IsEnabled = isEnabled;
        PlayButton.IsEnabled = isEnabled;
    }
}
