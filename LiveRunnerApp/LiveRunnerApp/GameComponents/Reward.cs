using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Reward : Sprite
{
    public Reward()
    {
        CollisionRegion = new SKRegion(new SKRectI(-15, -15, 15, 15));
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);

        // TODO: use a real sprite
        canvas.DrawCircle(Origin, 15, new() { Color = SKColors.Gold });
    }
}
