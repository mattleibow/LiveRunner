using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public partial class Game : ObservableObject
{
    private const int _gamePadding = 48;
    private const int _runSpeed = 500; // px per second
    private const int _laneCount = 3;
    private const double _newSpriteTime = 0.333;

    private int _lastUpdate;
    private double _lastSpriteAdd;

    private readonly Player _player = new();
    private readonly List<Sprite> _sprites = new();

    public Game()
    {
    }

    [ObservableProperty]
    private int _livesRemaining = 3;

    [ObservableProperty]
    private TimeSpan _timeElapsed;

    [ObservableProperty]
    private int _score;

    public void MovePlayer(MoveDirection direction)
    {
        if (direction == MoveDirection.Left && _player.Lane.Desired > 0)
            _player.Lane.Desired--;
        else if (direction == MoveDirection.Right && _player.Lane.Desired < _laneCount - 1)
            _player.Lane.Desired++;
        // TODO: add a beep sound if we move in a bad way
    }

    public void Update(int width, int height)
    {
        var deltaTime = UpdateElapsedTime();

        SpawnNewObstacle(deltaTime);

        var laneWidth = width / _laneCount;

        // update sprite locations
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            sprite.Update(deltaTime);

            var newY = sprite.Origin.Y + (_runSpeed * deltaTime);
            sprite.Origin = new(
                (int)(laneWidth * (sprite.Lane.Current + 1)) - laneWidth / 2,
                (float)newY);

            // sprite has moved off the edge, so remove it
            if (sprite.Origin.Y > height)
            {
                _sprites.RemoveAt(i);
            }
        }

        // update player location
        _player.Update(deltaTime);

        _player.Origin = new(
            (int)(laneWidth * (_player.Lane.Current + 1)) - laneWidth / 2,
            height - _gamePadding);

        // detect any collisions
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            if (sprite.Overlaps(_player))
            {
                if (sprite is Obstacle)
                {
                    LivesRemaining--;
                }

                _sprites.RemoveAt(i);
            }
        }
    }

    public void Draw(SKCanvas canvas, int width, int height)
    {
        canvas.Clear(SKColors.Green);

        // draw sprites
        foreach (var sprite in _sprites)
        {
            sprite.Draw(canvas, width, height);
        }

        // draw player
        _player.Draw(canvas, width, height);
    }

    private void SpawnNewObstacle(double deltaTime)
    {
        _lastSpriteAdd += deltaTime;

        // if we are still in the last add time, so bail out
        if (_lastSpriteAdd <= _newSpriteTime)
            return;

        // it has been X seconds since we added something, so add another

        _lastSpriteAdd = 0;

        _sprites.Add(new Obstacle { Lane = new(Random.Shared.Next(3)) });
    }

    private double UpdateElapsedTime()
    {
        if (_lastUpdate == 0)
            _lastUpdate = Environment.TickCount;

        var currentUpdate = Environment.TickCount;
        var deltaTime = (currentUpdate - _lastUpdate) / 1000.0;

        _lastUpdate = currentUpdate;

        TimeElapsed += TimeSpan.FromSeconds(deltaTime);

        return deltaTime;
    }
}
