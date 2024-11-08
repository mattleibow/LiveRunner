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

    public override SKImage? Asset => AssetManager.Default[GameAssets.Player];

    public override float AssetScale => 0.6f;

    public override SKPoint OriginOffset => new(-300, -600);
}
