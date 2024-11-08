using SkiaSharp;

namespace LiveRunnerApp.Controls;

public class GameSurfacePaintEventArgs : EventArgs
{
    public GameSurfacePaintEventArgs(SKCanvas canvas, SKSize size)
    {
        Canvas = canvas;
        Size = new((int)size.Width, (int)size.Height);
    }

    public SKCanvas Canvas { get; }

    public SKSizeI Size { get; }
}
