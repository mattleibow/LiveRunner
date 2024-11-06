using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace LiveRunnerApp;

public partial class SandboxPage : ContentPage
{
    private IDispatcherTimer? _timer;
    private SKImage? floorboards;
    private SKImage? bottle;
    private SKImage? player;

    public SandboxPage()
    {
        InitializeComponent();
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is null)
        {
            _timer?.Stop();
            _timer = null;
        }
        else
        {
            _timer = Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0);
            _timer.IsRepeating = true;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        floorboards ??= await LoadImageAsset("floorboards.jpg");
        bottle ??= await LoadImageAsset("bottle.png");
        player ??= await LoadImageAsset("player.png");
    }

    private static async Task<SKImage> LoadImageAsset(string name)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(name);
        return SKImage.FromEncodedData(stream);
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
#if WINDOWS
        if (gameSurface.Handler?.PlatformView is not Microsoft.UI.Xaml.FrameworkElement fe || fe.DispatcherQueue is null)
            return;
#endif

        _offset += 30;
        _offset = _offset % 256;

        _bottleOffset += 30;
        _bottleOffset = _bottleOffset % (256 * 10);

        gameSurface.InvalidateSurface();
    }

    private float _offset = 0;
    private float _bottleOffset = 0;

    private void OnDrawGame(object sender, SKPaintSurfaceEventArgs e)
    {
        const int tileSize = 256;
        var distStart = tileSize * -20;
        var distEnd = tileSize * 5;
        var laneStart = tileSize * -1;
        var laneEnd = tileSize * 1;

        if (floorboards is null || bottle is null || player is null)
            return;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.White);

        using var paint = new SKPaint();

        // scale to a nice size
        var pad = e.Info.Width / 5f;
        canvas.Translate(pad, e.Info.Height - pad - pad);
        canvas.Scale(0.4f, 0.4f);

        // rotate in 3D space
        var matrix = SKMatrix44.Concat(
            SKMatrix44.CreateRotationDegrees(0, 0, 1, 26),
            SKMatrix44.CreateRotationDegrees(1, 0, 0, 50));

        // draw floor tiles
        using (new SKAutoCanvasRestore(canvas))
        {
            canvas.Concat(matrix);
            canvas.Translate(tileSize / 2f, tileSize / 2f + _offset);

            for (var dist = distStart; dist <= distEnd; dist += tileSize)
            {
                for (var lane = laneStart; lane <= laneEnd; lane += tileSize)
                {
                    canvas.DrawImage(floorboards, lane, dist, paint);
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
            canvas.DrawImage(bottle, iso, paint);
        }

        // draw bottle
        using (new SKAutoCanvasRestore(canvas))
        {
            var scale = 0.7f;
            var iso = matrix.MapPoint(0, 0);
            canvas.DrawCircle(iso, 10, paint);

            canvas.Scale(scale);
            canvas.Translate(-300, -600);
            canvas.DrawImage(player, iso, paint);
        }
    }
}