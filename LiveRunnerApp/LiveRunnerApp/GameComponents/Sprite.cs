using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public class Sprite
{
    public AnimatedValue<double> Lane { get; set; } = new(0);

    public SKPoint Location { get; set; }

    public virtual void Update(double deltaTime)
    {
        Lane.Update(deltaTime);
    }

    public virtual void Draw(SKCanvas canvas, int width, int height)
    {
    }
}
