﻿using SkiaSharp;

namespace LiveRunnerEngine;

class Reward : Sprite
{
    public Reward()
    {
        CollisionRegion = new SKRegion(new SKRectI(-15, -15, 15, 15));
    }

    public override SKImage? Asset => AssetManager.Default[Assets.Bottle];

    public override float AssetScale => 0.4f;

    public override SKPoint OriginOffset => new(-128, -416);
}
