using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Player : Sprite
{
    public Player()
    {
        Lane.Duration = 0.3;
        Lane.Easing = Easing.SpringOut;

        CollisionRegion = new SKRegion(new SKRectI(-20, -20, 20, 20));
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);

        // TODO: use a real sprite
        canvas.DrawCircle(Origin, 20, new() { Color = SKColors.Blue });
    }
}
