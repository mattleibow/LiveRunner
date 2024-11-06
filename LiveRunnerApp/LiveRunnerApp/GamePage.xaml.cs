using CommunityToolkit.Maui.Views;
using LiveRunnerApp.GameComponents;
using SkiaSharp.Views.Maui;

namespace LiveRunnerApp;

public partial class GamePage : ContentPage
{
    private IDispatcherTimer? _timer;
    private Game _game = new();

    public GamePage()
    {
        InitializeComponent();

        BindingContext = _game;

        _game.GameOver += OnGameOver;
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

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                _game.MovePlayer(MoveDirection.Left);
                break;
            case SwipeDirection.Right:
                _game.MovePlayer(MoveDirection.Right);
                break;
        }
    }

    private async void OnGameOver(object? sender, EventArgs e)
    {
        await this.ShowPopupAsync(new GameOverPopup());
    }
}
