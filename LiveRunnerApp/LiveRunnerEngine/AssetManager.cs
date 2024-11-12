using SkiaSharp;

namespace LiveRunnerEngine;

public class AssetManager
{
    public static AssetManager Default { get; }

    static AssetManager()
    {
        Default = new();
        _ = Default.LoadAssets();
    }

    private Dictionary<string, SKImage> _assets = new();

    public bool IsLoaded { get; private set; }

    public SKImage this[string name] =>
        _assets.TryGetValue(name, out var asset)
            ? asset
            : throw new ArgumentException($"Asset '{name}' not found");

    public async Task LoadAssets()
    {
        await LoadImageAsset(Assets.FloorBoards);
        await LoadImageAsset(Assets.Bottle);
        await LoadImageAsset(Assets.Player);

        IsLoaded = true;
    }

    private async Task LoadImageAsset(string name)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(name);
        var image = SKImage.FromEncodedData(stream);
        _assets[name] = image;
    }
}
