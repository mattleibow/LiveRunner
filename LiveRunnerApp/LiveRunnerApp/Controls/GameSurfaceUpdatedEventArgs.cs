using SkiaSharp;

namespace LiveRunnerApp.Controls;

public class GameSurfaceUpdatedEventArgs : EventArgs
{
    public GameSurfaceUpdatedEventArgs(TimeSpan deltaTime, SKSize size)
    {
        DeltaTime = deltaTime;
        Size = new((int)size.Width, (int)size.Height);
    }

    public TimeSpan DeltaTime { get; }

    public SKSizeI Size { get; }
}
