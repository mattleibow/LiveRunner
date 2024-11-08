using SkiaSharp.Extended.UI.Controls;
using SkiaSharp;

namespace LiveRunnerApp.Controls;

public class GameSurface : SKAnimatedSurfaceView
{
    private SKSize _lastSize;

    public event EventHandler<GameSurfaceUpdatedEventArgs>? Updated;

    public event EventHandler<GameSurfacePaintEventArgs>? Paint;

    protected override void Update(TimeSpan deltaTime)
    {
        base.Update(deltaTime);

        Updated?.Invoke(this, new GameSurfaceUpdatedEventArgs(deltaTime, _lastSize));
    }

    protected override void OnPaintSurface(SKCanvas canvas, SKSize size)
    {
        base.OnPaintSurface(canvas, size);

        _lastSize = size;

        Paint?.Invoke(this, new GameSurfacePaintEventArgs(canvas, size));
    }
}
