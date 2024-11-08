using CommunityToolkit.Maui.Views;
using LiveRunnerApp.GameComponents;

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
        _game.RestartGame();

        //await this.ShowPopupAsync(new GameOverPopup());
    }

    private void OnFrameUpdate(object sender, Controls.GameSurfaceUpdatedEventArgs e)
    {
        _game.Update(e.DeltaTime, e.Size.Width, e.Size.Height);
    }

    private void OnFrameDraw(object sender, Controls.GameSurfacePaintEventArgs e)
    {
        _game.Draw(e.Canvas, e.Size.Width, e.Size.Height);
    }
}
