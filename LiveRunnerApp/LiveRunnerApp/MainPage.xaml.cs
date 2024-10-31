using LiveRunnerApp.GameComponents;
using SkiaSharp.Views.Maui;

namespace LiveRunnerApp;

public partial class MainPage : ContentPage
{
    private IDispatcherTimer? _timer;
    private Game _game = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            _timer?.Stop();
            _timer = null;
        }
        else
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0);
            _timer.IsRepeating = true;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
#if WINDOWS
        if (gameSurface.Handler?.PlatformView is not Microsoft.UI.Xaml.FrameworkElement fe || fe.DispatcherQueue is null)
            return;
#endif

        gameSurface.InvalidateSurface();
    }

    private void OnDrawGame(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;

        _game.Update(e.Info.Width, e.Info.Height);
        _game.Draw(canvas, e.Info.Width, e.Info.Height);
    }
}
