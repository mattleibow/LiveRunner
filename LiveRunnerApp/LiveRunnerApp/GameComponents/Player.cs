using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

class Player : Sprite
{
    public Player()
    {
    }

    public override void Draw(SKCanvas canvas, int width, int height)
    {
        base.Draw(canvas, width, height);

        // TODO: use a real sprite
        canvas.DrawCircle(Location, 20, new());
    }
}
