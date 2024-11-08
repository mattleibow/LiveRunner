using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System.Diagnostics;

namespace LiveRunnerApp.GameComponents;

public partial class Game : ObservableObject
{
    private const int _gamePadding = 48;
    private const int _runSpeed = 500; // px per second
    private const int _laneCount = 3;
    private const double _newSpriteTime = 0.333;
    private const int _tileSize = 256;

    private bool _isGameOver;

    private double _lastSpriteAdd;

    private readonly Player _player = new();
    private readonly List<Sprite> _sprites = new();

    private float _floorOffset = 0;

    [ObservableProperty]
    private int _livesRemaining = 3;

    [ObservableProperty]
    private TimeSpan _timeElapsed;

    [ObservableProperty]
    private int _score;

    public event EventHandler? GameOver;

    public void RestartGame()
    {
        _lastSpriteAdd = 0;
        _sprites.Clear();

        _floorOffset = 0;

        LivesRemaining = 3;
        TimeElapsed = TimeSpan.Zero;
        Score = 0;

        _isGameOver = false;
    }

    partial void OnLivesRemainingChanged(int value)
    {
        if (value == 0)
        {
            _isGameOver = true;
            GameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    public void MovePlayer(MoveDirection direction)
    {
        if (_isGameOver)
            return;

        if (direction == MoveDirection.Left && _player.Lane.Desired > 0)
            _player.Lane.Desired--;
        else if (direction == MoveDirection.Right && _player.Lane.Desired < _laneCount - 1)
            _player.Lane.Desired++;
        // TODO: add a beep sound if we move in a bad way
    }

    public void Update(TimeSpan deltaTimeSpan, int width, int height)
    {
        // TODO:
        width = _tileSize * 3;

        if (_isGameOver)
            return;

        var deltaTime = (float)deltaTimeSpan.TotalSeconds;

        var runDistance = _runSpeed * deltaTime;
        var laneWidth = width / _laneCount;

        // scrolling
        _floorOffset += runDistance;
        _floorOffset = _floorOffset % 256;

        // spawn a new sprite if we need to
        var newSprite = SpawnNewSprite(deltaTime);
        if (newSprite is not null)
        {
            newSprite.Origin = new(0, (1 / 0.4f) * -height);
        }

        // update sprite locations
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            sprite.Update(deltaTime);

            var newY = sprite.Origin.Y + runDistance;
            sprite.Origin = new(
                (int)(laneWidth * (sprite.Lane.Current + 1)) - laneWidth / 2,
                newY);

            // sprite has moved off the edge, so remove it
            if (sprite.Origin.Y > 0)
            {
                _sprites.RemoveAt(i);
            }
        }

        // update player location
        {
            _player.Update(deltaTime);

            _player.Origin = new(
                (int)(laneWidth * (_player.Lane.Current + 1)) - laneWidth / 2,
                 -laneWidth / 2);
        }

        // detect any collisions
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            if (sprite.Overlaps(_player))
            {
                _sprites.RemoveAt(i);

                if (sprite is Obstacle)
                {
                    //LivesRemaining--;
                }
                else if (sprite is Reward)
                {
                    Score++;
                }
            }
        }
    }

    float _angle = 0;

    public void Draw(SKCanvas canvas, int width, int height)
    {
        var distStart = _tileSize * -20;
        var distEnd = _tileSize * 5;
        var laneStart = _tileSize * -1;
        var laneEnd = _tileSize * 1;

        if (!AssetManager.Default.IsLoaded)
            return;

        canvas.Clear(SKColors.Green);

        using var paint = new SKPaint();

        // move the origin to the bottom left
        canvas.Translate(0, height*0.7f);

        // scale to a nice size
        canvas.Scale(0.25f, 0.25f);

        // rotate in 3D space
        var floorAngle = MathF.Atan((float)width / height);
        var deg = floorAngle * 180 / MathF.PI;
        var matrix = SKMatrix44.Concat(
            SKMatrix44.CreateRotationDegrees(0, 0, 1, deg - 10),
            SKMatrix44.CreateRotationDegrees(1, 0, 0, 45));

        // draw floor tiles
        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.Concat(matrix);
            canvas.Translate(_tileSize / 2, _floorOffset);

            for (var dist = distStart; dist <= distEnd; dist += _tileSize)
            {
                for (var lane = laneStart; lane <= laneEnd; lane += _tileSize)
                {
                    canvas.DrawImage(AssetManager.Default[GameAssets.FloorBoards], lane, dist, paint);
                }
            }

            paint.Color = SKColors.Red;
            canvas.DrawCircle(0, 0, 20, paint);
        }

        // draw sprites
        foreach (var sprite in _sprites)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                sprite.Draw(canvas, width, height, matrix);
            }
        }

        // draw player
        _player.Draw(canvas, width, height, matrix);
    }

    private Sprite? SpawnNewSprite(double deltaTime)
    {
        _lastSpriteAdd += deltaTime;

        // if we are still in the last add time, so bail out
        if (_lastSpriteAdd <= _newSpriteTime)
            return null;

        // it has been X seconds since we added something, so add another

        _lastSpriteAdd = 0;

        Sprite sprite;
        if (Random.Shared.Next(2) == 0)
            _sprites.Add(sprite = new Reward { Lane = new(Random.Shared.Next(3)) });
        else
            _sprites.Add(sprite = new Obstacle { Lane = new(Random.Shared.Next(3)) });
        return sprite;
    }
}
