using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Player : Sprite
{
    public Player()
    {
        Lane.Duration = 0.5;
        Lane.Easing = Easing.SpringOut;        
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);

        // TODO: use a real sprite
        canvas.DrawCircle(Location, 20, new() { Color = SKColors.Blue });
    }
}
