using Binaural.Beat.Application;
using Binaural.Beat.Infrastructure.Audio.NAudio;
using MahApps.Metro.Controls;

namespace Binaural.Beat.WpfClient;

public partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new BinauralBeatService(new NAudioPlaybackEngine()));
    }
}
