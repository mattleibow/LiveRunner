using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public class Game
{
    private const int _gamePadding = 48;
    private const int _runSpeed = 20;
    private const int _laneCount = 3;

    private SKPaint _linePaint = new()
    {
        StrokeWidth = 1,
        Style = SKPaintStyle.Stroke
    };

    private int _laneWidth;

    private readonly Player _player = new();
    private readonly List<Sprite> _sprites = new();

    public Game()
    {
        // TODO: temporary obstacles
        _sprites.Add(new Obstacle() { Lane = 0 });
        _sprites.Add(new Obstacle() { Lane = 2 });
    }

    public void Update(int width, int height)
    {
        _laneWidth = width / _laneCount;

        // update sprites
        foreach (var sprite in _sprites)
        {
            // TODO: speed should not be directly added
            //       and should be based on delta time
            var newY = sprite.Location.Y + _runSpeed;
            sprite.Location = new(
                _laneWidth * (sprite.Lane + 1) - _laneWidth / 2,
                newY);

            // TODO: sprite has moved off the edge, so remove it
            if (sprite.Location.Y > height)
                sprite.Location = new(sprite.Location.X, 0);
        }

        // update player
        _player.Location = new(
            _laneWidth * (_player.Lane + 1) - _laneWidth / 2,
            height - _gamePadding);
    }

    public void Draw(SKCanvas canvas, int width, int height)
    {
        canvas.Clear(SKColors.Green);

        // draw temp lane lines
        for (var i = 1; i < _laneCount; i++)
        {
            canvas.DrawLine(
                _laneWidth * i, 0,
                _laneWidth * i, height,
                _linePaint);
        }

        // draw sprites
        foreach (var sprite in _sprites)
        {
            sprite.Draw(canvas, width, height);
        }

        // draw player
        _player.Draw(canvas, width, height);

        // draw temp padding line
        canvas.DrawRect(
            SKRect.Inflate(SKRect.Create(width, height), -_gamePadding, -_gamePadding),
            _linePaint);
    }
}
