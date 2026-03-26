namespace p3rpc.asyncassetloader.Interfaces;

public interface IAssetLoader
{
    /// <summary>
    /// Add an asset at the specified path onto the asset queue. target is where the asset object's pointer will be written to
    /// and is assumed to be an allocated section of memory. Call `LoadQueuedAssets` to start actually loading the assets.
    /// </summary>
    /// <param name="loader">UAssetLoader instance (argument is UAssetLoader*)</param>
    /// <param name="path">Path to the asset. Must be in the format "/Game/[Asset Path]/[File Name].[File Name]"</param>
    /// <param name="target">A pointer to an allocated part of memory where the asset object will be asynchronously written to.
    /// This type is UObject**.</param>
    /// <param name="onLoaderCb">The callback which is run if the asset is found. It's argument is UObject**.</param>
    void LoadAsset(nint loader, string path, nint target, Action<nint> onLoaderCb);

    /// <summary>
    /// Tells the UAssetLoader to start asynchronously load assets defined from the asset path queue.
    /// </summary>
    /// <param name="loader">UAssetLoader instance (argument is UAssetLoader*)</param>
    void LoadQueuedAssets(nint loader);

    /// <summary>
    /// Creates a TSharedPtr&#60;FStreamableHandle&#62; for the UAssetLoader which allows it to load assets from the game
    /// with a matching path using LoadAsset. Call this after creating the object (`SpawnObject` in UE Toolkit)
    /// </summary>
    /// <param name="loader">UAssetLoader instance (argument is UAssetLoader*)</param>
    void CreateHandle(nint loader);
}