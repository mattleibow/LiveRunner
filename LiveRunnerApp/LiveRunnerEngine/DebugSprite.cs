using SkiaSharp;

namespace LiveRunnerEngine;

internal class DebugSprite : Sprite
{
    private readonly string _text;
    private readonly SKFont _font = new() { Size = 128 };
    private readonly SKPaint _paint = new() { Color = SKColors.Fuchsia };

    public DebugSprite(string text)
    {
        _text = text;
    }

    protected override void DrawAsset(SKCanvas canvas, int width, int height)
    {
        canvas.DrawText(_text, SKPoint.Empty,_font, _paint);
    }
}
