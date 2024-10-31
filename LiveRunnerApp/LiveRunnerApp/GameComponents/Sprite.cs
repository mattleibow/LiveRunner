using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Sprite
{
    public int Lane { get; set; }

    public SKPoint Location { get; set; }

    public virtual void Draw(SKCanvas canvas, int width, int height)
    {
    }
}
