using SkiaSharp;

namespace LiveRunnerEngine;

class Obstacle : Sprite
{
    public Obstacle()
    {
        CollisionRegion = new SKRegion(new SKRectI(-15, -15, 15, 15));
    }

    public override SKImage? Asset => AssetManager.Default[Assets.Bottle];

    public override float AssetScale => 0.5f;

    public override SKPoint OriginOffset => new(-128, -416);
}
