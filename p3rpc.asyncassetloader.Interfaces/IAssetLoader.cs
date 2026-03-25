namespace p3rpc.asyncassetloader.Interfaces;

public unsafe interface IAssetLoader
{
    // target: UObject**,
    // onLoaderCb: parameter passed is UObject**
    void LoadAsset(nint loader, string path, nint target, Action<nint> onLoaderCb);

    void LoadQueuedAssets(nint loader);

    void CreateHandle(nint loader);
}