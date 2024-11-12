using CommunityToolkit.Mvvm.ComponentModel;
using SkiaSharp;

namespace LiveRunnerEngine;

public partial class Game : ObservableObject
{
    private const int DefaultLivesRemaning = 3;

    /// <summary>
    /// The base running speed in pixels per second (px/s).
    /// </summary>
    private const int RunningSpeed = 600;

    /// <summary>
    /// The number of lanes.
    /// </summary>
    private const int LaneCount = 3;

    /// <summary>
    /// The base size of a tile/lane/sprite.
    /// </summary>
    private const int TileSize = 256;

    /// <summary>
    /// The global scale to apply to ensure the game is small enough to be played since the assets are large.
    /// </summary>
    private const float GlobalScale = 0.3f;

    /// <summary>
    /// The distance from the bottom of the screen to place the player sprite.
    /// </summary>
    private const int VerticalPlayerOffset = -2 * TileSize;

    /// <summary>
    /// The distance from the left of the screen to place the player sprite.
    /// </summary>
    private const int HorizontalPlaterOffset = 0;

    /// <summary>
    /// The number of times to render behind the player sprite.
    /// </summary>
    private const int TileCountBehindPlayer = TileSize * 5;

    /// <summary>
    /// The manager which picks chunks for the game.
    /// </summary>
    private ChunkManager _chunkManager = new(LaneCount);

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
    private int _livesRemaining = DefaultLivesRemaning;

    [ObservableProperty]
    private TimeSpan _timeElapsed;

    [ObservableProperty]
    private int _score;

    public event EventHandler? GameOver;

    public void RestartGame()
    {
        _chunkManager = new ChunkManager(LaneCount);

        _sprites.Clear();

        _floorOffset = 0;

        LivesRemaining = DefaultLivesRemaning;
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
        else if (direction == MoveDirection.Right && _player.Lane.Desired < LaneCount - 1)
            _player.Lane.Desired++;

        // TODO: add a beep sound if we move in a bad way
    }

    public void Update(TimeSpan deltaTimeSpan, int width, int height)
    {
        if (_isGameOver)
            return;

        var deltaTime = (float)deltaTimeSpan.TotalSeconds;

        var runDistance = RunningSpeed * deltaTime;
        var laneWidth = TileSize;

        // scroll the floor
        _floorOffset = (_floorOffset + runDistance) % TileSize;

        // calculate the 2.5D transformation
        var floorAngle = MathF.Atan((float)width / height);
        var deg = floorAngle * 180 / MathF.PI;
        _isometricMatrix =
            SKMatrix44.CreateRotationDegrees(0, 0, 1, deg - 10) *
            SKMatrix44.CreateRotationDegrees(1, 0, 0, 45);
        _originMatrix =
            // move the player away from the edge a bit
            SKMatrix44.CreateTranslation(HorizontalPlaterOffset, VerticalPlayerOffset, 0) *
            // scale smaller to fit nicely
            SKMatrix44.CreateScale(GlobalScale, GlobalScale, 1) *
            // move the origin to the bottom left of the screen
            SKMatrix44.CreateTranslation(0, height, 0);

        // calculate the visible floor length
        var invertedMatrix = (_isometricMatrix * _originMatrix).Invert();
        var topLeftCoordinate = invertedMatrix.MapPoint(width, -height);
        _visibleFloorLength = -topLeftCoordinate.Y;

        // spawn a new sprite if we need to
        SpawnNewSprite(deltaTime);

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
            if (sprite.Origin.Y > TileCountBehindPlayer)
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
        canvas.Clear(SKColors.Black);

        // DEBUG: zoom out and draw the window border
        if (DebugOptions.ScaleEntireGame)
        {
            canvas.Scale(0.5f, 0.5f, width / 2, height / 2);
            canvas.DrawRect(0, 0, width, height, new() { Style = SKPaintStyle.Stroke, Color = SKColors.Fuchsia });
        }

        if (!AssetManager.Default.IsLoaded)
            return;

        var distNear = TileCountBehindPlayer;
        var distFar = -_visibleFloorLength;
        var laneStart = TileSize * 0;
        var laneEnd = TileSize * (LaneCount - 1);

        // apply the origin transformation
        canvas.Concat(_originMatrix);

        // add some padding
        canvas.Translate(TileSize / 2, TileSize * -1);

        // draw floor tiles
        using (new SKAutoCanvasRestore(canvas))
        {
            // apply the 2.5D transformation
            canvas.Concat(_isometricMatrix);

            // animate the floor
            canvas.Translate(0, _floorOffset);

            // TODO: draw the back wall
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.Concat(SKMatrix44.CreateRotationDegrees(0, 1, 0, 90));
                canvas.Concat(SKMatrix44.CreateTranslation(-4 * TileSize, 0, 0));
                canvas.Concat(SKMatrix44.CreateScale(4, 1, 1));

                for (var dist = distFar; dist <= distNear; dist += TileSize)
                {
                    canvas.DrawImage(AssetManager.Default[Assets.FloorBoards], 0, dist);
                }
            }

            // TODO: draw the front wall
            using (new SKAutoCanvasRestore(canvas))
            {
                canvas.Concat(SKMatrix44.CreateRotationDegrees(0, 1, 0, 90));
                canvas.Concat(SKMatrix44.CreateTranslation(0, 0, TileSize * 3));
                canvas.Concat(SKMatrix44.CreateScale(4, 1, 1));

                for (var dist = distFar; dist <= distNear; dist += TileSize)
                {
                    canvas.DrawImage(AssetManager.Default[Assets.FloorBoards], 0, dist);
                }
            }

            // draw the floor
            for (var dist = distFar; dist <= distNear; dist += TileSize)
            {
                for (var lane = laneStart; lane <= laneEnd; lane += TileSize)
                {
                    canvas.DrawImage(AssetManager.Default[Assets.FloorBoards], lane, dist);
                }
            }
        }

        // draw sprites in front of the player (from far to close)
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            if (sprite.Origin.Y > 0)
                continue;

            sprite.Draw(canvas, width, height, _isometricMatrix);
        }

        // draw player
        {
            _player.Draw(canvas, width, height, _isometricMatrix);
        }

        // draw sprites behind the player (from far to close)
        for (var i = _sprites.Count - 1; i >= 0; i--)
        {
            var sprite = _sprites[i];

            if (sprite.Origin.Y <= 0)
                continue;

            sprite.Draw(canvas, width, height, _isometricMatrix);
        }
    }

    private void SpawnNewSprite(double deltaTime)
    {
        if (!ShouldSpawnNewSprite())
            return;

        // the furthest sprite is on screen, so we need to
        // add a new chunk of sprites

        var nextChunk = _chunkManager.PickNextChunk();

        var debugSprite = DebugOptions.DrawChunkNames
            ? new DebugSprite(nextChunk.Name ?? "<null chunk>")
            : null;

        _sprites.Add(debugSprite);

        // add the sprites to the game, starting from the first sprite
        // in the nearest lane (because we draw backwards)
        for (var laneNumber = nextChunk.Sprites.Length - 1; laneNumber >= 0; laneNumber--)
        {
            var lane = nextChunk.Sprites[laneNumber];

            for (var spriteNumber = 0; spriteNumber < lane.Length; spriteNumber++)
            {
                var spriteKind = lane[spriteNumber];

                // create the sprite
                var sprite = spriteKind switch
                {
                    SpriteKind.Reward => new Reward(),
                    SpriteKind.Obstacle => new Obstacle(),
                    _ => (Sprite?)null
                };

                if (sprite is null)
                    continue;

                sprite.Lane = new(laneNumber);
                sprite.Origin = new(0, -_visibleFloorLength - spriteNumber * TileSize);

                _sprites.Add(sprite);
            }
        }
    }

    private bool ShouldSpawnNewSprite()
    {
        // there are nos sprites, so we need something
        if (_sprites.Count == 0)
            return true;

        // find the furthest sprite that was added before
        var furthestSprite = _sprites.Min(s => s.Origin.Y);

        // if the furthest sprite is still off the screen, do nothing
        if (furthestSprite < -(_visibleFloorLength - TileSize))
            return false;

        // we need more sprites
        return true;
    }
}
