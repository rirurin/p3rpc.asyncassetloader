# Async Asset Loader for Persona 3 Reload

## Purpose

Exposes methods for interacting with Persona 3 Reload's `UAssetLoader` to asynchronously load assets from the game.

This was originally part of a framework I was making to add new social links into the game (Saori :true:) but I also needed it when adding custom party panel textures for [Costume Framework](https://github.com/RyoTune/P3R.CostumeFramework).

## Usage

To obtain a `UAssetLoader` instance, you can either hook something that already has one or create your own using
[Unreal Toolkit](https://github.com/RyoTune/UE.Toolkit):

```c#
// where toolkitSpawning is an instance of IUnrealSpawning
// and assetLoader is an instance of IAssetLoader
SampleAssetLoader = toolkitSpawning.SpawnObject<UAssetLoader>("SampleAssetLoader", null);
assetLoader.CreateHandle(CostumeFrameworkAssetLoader.Ptr);
```

After creating your object, call `IAssetLoader.CreateHandle`. This will create a `TSharedPtr<FStreamableHandle>`, which will allow UAssetLoader to access the game's files. Without this, all load requests will fail.

*Note that all functions in `IAssetLoader` pass in an nint instead of `UAssetLoader*`*

To add an asset onto the loading queue, use `IAssetLoader.LoadAsset`:

```c#
assetLoader.LoadAsset(
    SampleAssetLoader!.Ptr, "/Game/SamplePath/FileName.FileName", Target,
    // This callback will run if the file loaded successfully.
    x =>
    {
        // Prevents the object from being garbage collected. You can also set the
        // parent object so it's lifetime is linked to the parent.
        toolkitObjects.GUObjectArray.AddToRootSet((*(UObjectBase**)x)->InternalIndex);
    });
```

Once you've queued the assets you want to load, call `IAssetLoader.LoadQueuedAssets`:

```c#
assetLoader.LoadQueuedAssets(SampleAssetLoader.Ptr);
```