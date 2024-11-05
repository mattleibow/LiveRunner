using Microsoft.Maui.Layouts;
using SkiaSharp;

namespace LiveRunnerApp.GameComponents;

public class Sprite
{
    private SKPoint _origin;
    private SKRegion? _shape;
    private SKRegion? _shapeAtOrigin;

    public AnimatedValue<double> Lane { get; set; } = new(0);

    public SKPoint Origin
    {
        get => _origin;
        set
        {
            _origin = value;
            _shapeAtOrigin?.Dispose();
            _shapeAtOrigin = null;
        }
    }

    public SKRegion? Shape
    {
        get => _shape;
        set
        {
            _shape = value;
            _shapeAtOrigin?.Dispose();
            _shapeAtOrigin = null;
        }
    }

    public SKRegion ShapeAtOrigin
    {
        get
        {
            if (_shapeAtOrigin is null)
            {
                var atOrigin = new SKRegion(Shape);
                atOrigin.Translate((int)Origin.X, (int)Origin.Y);
                _shapeAtOrigin = atOrigin;
            }
            return _shapeAtOrigin;
        }
    }

    public virtual void Update(double deltaTime)
    {
        Lane.Update(deltaTime);
    }

    public virtual void Draw(SKCanvas canvas, int width, int height)
    {
    }

    public virtual bool Overlaps(Sprite sprite)
    {
        if (Shape is null || sprite.Shape is null)
            return false;

        //var shapeBounds = (SKRect)Shape.Bounds;
        //shapeBounds.Offset(Origin);

        if (ShapeAtOrigin.Intersects(sprite.ShapeAtOrigin))
            return true;

        return false;
    }
}
