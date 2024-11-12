using CommunityToolkit.Maui.Views;
using LiveRunnerApp.Controls;
using LiveRunnerEngine;

namespace LiveRunnerApp;

public partial class GamePage : ContentPage
{
    private Game _game = new();

    public GamePage()
    {
        InitializeComponent();

        BindingContext = _game;

        _game.GameOver += OnGameOver;
    }

    private void OnSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction.HasFlag(SwipeDirection.Left))
            _game.MovePlayer(MoveDirection.Left);
        else if (e.Direction.HasFlag(SwipeDirection.Right))
            _game.MovePlayer(MoveDirection.Right);
    }

    private async void OnGameOver(object? sender, EventArgs e)
    {
        _game.RestartGame();

        //await this.ShowPopupAsync(new GameOverPopup());
    }

    private void OnFrameUpdate(object sender, GameSurfaceUpdatedEventArgs e)
    {
        _game.Update(e.DeltaTime, e.Size.Width, e.Size.Height);
    }

    private void OnFrameDraw(object sender, GameSurfacePaintEventArgs e)
    {
        _game.Draw(e.Canvas, e.Size.Width, e.Size.Height);
    }

    private void OnTapped(object sender, TappedEventArgs e)
    {
        var relative = e.GetPosition(gameSurface);
        if (relative?.X is not { } x)
            return;

        var width = gameSurface.Width;

        if (x < width * 0.3333)
            _game.MovePlayer(MoveDirection.Left);
        else if (x > width * 0.6667)
            _game.MovePlayer(MoveDirection.Right);
    }
}
