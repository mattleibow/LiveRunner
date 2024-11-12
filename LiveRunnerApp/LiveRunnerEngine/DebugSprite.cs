using SkiaSharp;

namespace LiveRunnerEngine;

internal class DebugSprite : Sprite
{
    private readonly string _text;

    public DebugSprite(string text)
    {
        _text = text;
    }

    protected override void DrawAsset(SKCanvas canvas, int width, int height)
    {
        canvas.DrawText(_text, SKPoint.Empty, new SKFont(), new SKPaint { Color = SKColors.Fuchsia });
    }
}
