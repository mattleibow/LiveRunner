using Microsoft.Maui.Layouts;
using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public class Sprite
{
    private SKPoint _origin;
    private SKRegion? _collisionRegion;
    private SKRegion? _collisionRegionAtOrigin;

    public AnimatedValue<double> Lane { get; set; } = new(0);

    public SKPoint Origin
    {
        get => _origin;
        set
        {
            _origin = value;
            _collisionRegionAtOrigin?.Dispose();
            _collisionRegionAtOrigin = null;
        }
    }

    public SKRegion? CollisionRegion
    {
        get => _collisionRegion;
        set
        {
            _collisionRegion = value;
            _collisionRegionAtOrigin?.Dispose();
            _collisionRegionAtOrigin = null;
        }
    }

    public SKRegion CollisionRegionAtOrigin
    {
        get
        {
            if (_collisionRegionAtOrigin is null)
            {
                var atOrigin = new SKRegion(CollisionRegion);
                atOrigin.Translate((int)Origin.X, (int)Origin.Y);
                _collisionRegionAtOrigin = atOrigin;
            }
            return _collisionRegionAtOrigin;
        }
    }

    public virtual SKImage? Asset { get; }

    public virtual float AssetScale { get; }

    public virtual SKPoint OriginOffset { get; }

    public virtual void Update(double deltaTime)
    {
        Lane.Update(deltaTime);
    }

    public virtual void Draw(SKCanvas canvas, int width, int height, SKMatrix44 transform)
    {
        if (Asset is null)
            return;

        using var _ = new SKAutoCanvasRestore(canvas);

        // move canvas origin to sprite origin
        var coord = transform.MapPoint(Origin);
        canvas.Translate(coord);

        // scale sprite
        canvas.Scale(AssetScale);

        // offset the sprite so that the "center of mass" is at the origin
        canvas.Translate(OriginOffset);

        // draw the sprite at the transformed canvas origin
        canvas.DrawImage(Asset, SKPoint.Empty);
    }

    public virtual bool Overlaps(Sprite sprite)
    {
        if (CollisionRegion is null || sprite.CollisionRegion is null)
            return false;

        //var shapeBounds = (SKRect)Shape.Bounds;
        //shapeBounds.Offset(Origin);

        if (CollisionRegionAtOrigin.Intersects(sprite.CollisionRegionAtOrigin))
            return true;

        return false;
    }
}
