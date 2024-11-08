using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public partial class Game : ObservableObject
{
    /// <summary>
    /// The base running speed in pixels per second (px/s).
    /// </summary>
    private const int _runSpeed = 500;

    /// <summary>
    /// The number of lanes.
    /// </summary>
    private const int _laneCount = 3;

    /// <summary>
    /// HACK: The number of seconds to wait before spawning a new sprite.
    /// </summary>
    private const double _newSpriteTime = 0.333;

    /// <summary>
    /// The base size of a tile/lane/sprite.
    /// </summary>
    private const int _tileSize = 256;

    /// <summary>
    /// The global scale to apply to ensure the game is small enough to be played since the assets are large.
    /// </summary>
    private const float _globalScale = 0.3f;

    /// <summary>
    /// The player sprite.
    /// </summary>
    private readonly Player _player = new();

    /// <summary>
    /// The moving sprites.
    /// </summary>
    private readonly List<Sprite> _sprites = new();

    /// <summary>
    /// A boolean flag that indicates whether or not the game is over.
    /// </summary>
    private bool _isGameOver;

    /// <summary>
    /// HACK: The last time we added a sprite.
    /// </summary>
    private double _lastSpriteAdd;

    /// <summary>
    /// The offset to animate the moving floor.
    /// </summary>
    private float _floorOffset = 0;

    /// <summary>
    /// The matrix to transform the 2D game into 2.5D.
    /// </summary>
    private SKMatrix44 _isometricMatrix = SKMatrix44.Identity;

    /// <summary>
    /// The matrix to use as the base origin.
    /// </summary>
    private SKMatrix44 _originMatrix = SKMatrix44.Identity;

    /// <summary>
    /// The length of the visible part of the floor.
    /// </summary>
    private float _visibleFloorLength = 0;

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
        if (_isGameOver)
            return;

        var deltaTime = (float)deltaTimeSpan.TotalSeconds;

        var runDistance = _runSpeed * deltaTime;
        var laneWidth = _tileSize;

        // scroll the floor
        _floorOffset = (_floorOffset + runDistance) % _tileSize;

        // calculate the 2.5D transformation
        var floorAngle = MathF.Atan((float)width / height);
        var deg = floorAngle * 180 / MathF.PI;
        _isometricMatrix =
            SKMatrix44.CreateRotationDegrees(0, 0, 1, deg - 10) *
            SKMatrix44.CreateRotationDegrees(1, 0, 0, 45);
        _originMatrix =
            // scale smaller to fit nicely
            SKMatrix44.CreateScale(_globalScale, _globalScale, 1) *
            // move the origin to the bottom left of the screen
            SKMatrix44.CreateTranslation(0, height, 0);

        // calculate the visible floor length
        var invertedMatrix = (_isometricMatrix * _originMatrix).Invert();
        var topLeftCoordinate = invertedMatrix.MapPoint(width, -height);
        _visibleFloorLength = -topLeftCoordinate.Y;

        // spawn a new sprite if we need to
        var newSprite = SpawnNewSprite(deltaTime);
        if (newSprite is not null)
        {
            newSprite.Origin = new(0, -_visibleFloorLength);
        }

        // update sprite locations
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            sprite.Update(deltaTime);

            // make sure the origin is in the center of the right lane
            // and translated down toward the player
            sprite.Origin = new(
                (int)(laneWidth * sprite.Lane.Current) + laneWidth / 2,
                sprite.Origin.Y + runDistance);

            // sprite has moved off the edge, so remove it
            if (sprite.Origin.Y > 0)
            {
                _sprites.RemoveAt(i);
            }
        }

        // update player location
        {
            _player.Update(deltaTime);

            // make sure the origin is in the center of the right lane
            // and still at the bottom of the screen
            _player.Origin = new(
                (int)(laneWidth * _player.Lane.Current) + laneWidth / 2,
                0);
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

    public void Draw(SKCanvas canvas, int width, int height)
    {
        var distNear = _tileSize * 3;
        var distFar = -_visibleFloorLength;
        var laneStart = _tileSize * 0;
        var laneEnd = _tileSize * (_laneCount - 1);

        canvas.Clear(SKColors.Green);

        // DEBUG: zoom out and draw the window border
        //canvas.Scale(0.5f, 0.5f, width / 2, height / 2);
        //canvas.DrawRect(0, 0, width, height, new() { Style = SKPaintStyle.Stroke });

        if (!AssetManager.Default.IsLoaded)
            return;

        // apply the origin transformation
        canvas.Concat(_originMatrix);

        // add some padding
        canvas.Translate(_tileSize / 2, _tileSize * -1);

        // draw floor tiles
        using (new SKAutoCanvasRestore(canvas))
        {
            // apply the 2.5D transformation
            canvas.Concat(_isometricMatrix);
            
            // animate the floor
            canvas.Translate(0, _floorOffset);

            for (var dist = distFar; dist <= distNear; dist += _tileSize)
            {
                for (var lane = laneStart; lane <= laneEnd; lane += _tileSize)
                {
                    canvas.DrawImage(AssetManager.Default[GameAssets.FloorBoards], lane, dist);
                }
            }
        }

        // draw sprites
        foreach (var sprite in _sprites)
        {
            sprite.Draw(canvas, width, height, _isometricMatrix);
        }

        // draw player
        {
            _player.Draw(canvas, width, height, _isometricMatrix);
        }
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
