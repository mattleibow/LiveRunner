using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Obstacle : Sprite
{
    public Obstacle()
    {
        CollisionRegion = new SKRegion(new SKRectI(-15, -15, 15, 15));
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);

        // TODO: use a real sprite
        canvas.DrawCircle(Origin, 15, new());
    }
}
