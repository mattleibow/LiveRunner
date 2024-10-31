using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Obstacle : Sprite
{
    public Obstacle()
    {
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);
    
        // TODO: use a real sprite
        canvas.DrawCircle(Location, 15, new());
    }
}
