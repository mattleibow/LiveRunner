using LiveRunnerApp.Controls;
using LiveRunnerEngine;
using SkiaSharp;

namespace LiveRunnerApp;

public partial class SandboxPage : ContentPage
{
    public SandboxPage()
    {
        InitializeComponent();
    }

    private float _floorOffset = 0;
    private float _bottleOffset = 0;

    private void OnFrameUpdate(object sender, GameSurfaceUpdatedEventArgs e)
    {
        _floorOffset += 30;
        _floorOffset = _floorOffset % 256;

        _bottleOffset += 30;
        _bottleOffset = _bottleOffset % (256 * 10);
    }

    private void OnFrameDraw(object sender, GameSurfacePaintEventArgs e)
    {
        const int tileSize = 256;
        var distStart = tileSize * -20;
        var distEnd = tileSize * 5;
        var laneStart = tileSize * -1;
        var laneEnd = tileSize * 1;

        if (!AssetManager.Default.IsLoaded)
            return;

        var canvas = e.Canvas;
        canvas.Clear(SKColors.White);

        using var paint = new SKPaint();

        // scale to a nice size
        var pad = e.Size.Width / 5f;
        canvas.Translate(pad, e.Size.Height - pad - pad);
        canvas.Scale(0.4f, 0.4f);

        // rotate in 3D space
        var matrix = SKMatrix44.Concat(
            SKMatrix44.CreateRotationDegrees(0, 0, 1, 26),
            SKMatrix44.CreateRotationDegrees(1, 0, 0, 50));

        // draw floor tiles
        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.Concat(matrix);
            canvas.Translate(tileSize / 2f, tileSize / 2f + _floorOffset);

            for (var dist = distStart; dist <= distEnd; dist += tileSize)
            {
                for (var lane = laneStart; lane <= laneEnd; lane += tileSize)
                {
                    canvas.DrawImage(AssetManager.Default[Assets.FloorBoards], lane, dist, paint);
                }
            }
        }

        // draw bottle
        using (new SKAutoCanvasRestore(canvas))
        {
            var scale = 0.5f;
            var iso = matrix.MapPoint(0, (_bottleOffset - (256 * 10)) / scale);
            canvas.Scale(scale);
            canvas.Translate(-128, -416);
            canvas.DrawImage(AssetManager.Default[Assets.Bottle], iso, paint);
        }

        // draw bottle
        using (new SKAutoCanvasRestore(canvas))
        {
            var scale = 0.7f;
            var iso = matrix.MapPoint(0, 0);
            canvas.DrawCircle(iso, 10, paint);

            canvas.Scale(scale);
            canvas.Translate(-300, -600);
            canvas.DrawImage(AssetManager.Default[Assets.Player], iso, paint);
        }
    }
}
