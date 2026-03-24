using p3rpc.asyncassetloader.Interfaces;
using p3rpc.commonmodutils;
using p3rpc.nativetypes.Interfaces;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using RyoTune.Reloaded;
using UE.Toolkit.Core.Types.Unreal.UE5_4_4;
using FString = p3rpc.nativetypes.Interfaces.FString;

namespace p3rpc.asyncassetloader;

// ReSharper disable once ClassNeverInstantiated.Global
public class AssetLoader : ModuleBase<AssetContext>, IAssetLoader
{
    private string UAssetLoader_CheckStreamedAssets_SIG = "49 8D 4F ?? 48 89 5C 24 ?? 48 8D 54 24 ??";
    private string UAssetLoader_CheckStreamedAssets_SIG_EpAigis = "49 8D 0C ?? 48 89 5C 24 ?? 48 8D 54 24 ??";
    private MultiSignature CheckStreamedAssetsMS;
    private IAsmHook _checkStreamedAssets;
    private IReverseWrapper<UAssetLoader_CheckStreamedAssets> _checkStreamedAssetsWrapper;
    [Function(FunctionAttribute.Register.r15, FunctionAttribute.Register.rax, false)]
    public unsafe delegate void UAssetLoader_CheckStreamedAssets(UAssetLoader* loader);

    private SHFunction<UAssetLoader_LoadTargetAsset> _loadTargetAsset;
    private unsafe delegate void UAssetLoader_LoadTargetAsset(UAssetLoader* self, FString* name, nint dest);

    private SHFunction<UAssetLoader_LoadQueuedAssets> _loadQueuedAssets;
    private unsafe delegate void UAssetLoader_LoadQueuedAssets(UAssetLoader* self);

    private Dictionary<nint, (Action<nint> onLoadCb, string fileName)> MemoryToNotify = new();

    public unsafe AssetLoader(AssetContext context, Dictionary<string, ModuleBase<AssetContext>> modules) : base(context, modules)
    {
        CheckStreamedAssetsMS = new MultiSignature();
        _context._utils.MultiSigScan(
            [UAssetLoader_CheckStreamedAssets_SIG, UAssetLoader_CheckStreamedAssets_SIG_EpAigis],
            "UAssetLoader::CheckStreamedAssets", _context._utils.GetDirectAddress,
            addr =>
            {
                string[] function =
                [
                    "use64",
                    $"{_context._utils.PreserveMicrosoftRegisters()}",
                    $"{_context._hooks.Utilities.GetAbsoluteCallMnemonics(UAssetLoader_CheckStreamedAssetsImpl, out _checkStreamedAssetsWrapper)}",
                    $"{_context._utils.RetrieveMicrosoftRegisters()}"
                ];
                _checkStreamedAssets = _context._hooks.CreateAsmHook(function, addr).Activate();
            },
            CheckStreamedAssetsMS
        );
        _loadTargetAsset = new();
        _loadQueuedAssets = new();
    }

    public override void Register() {}

    private unsafe void UAssetLoader_CheckStreamedAssetsImpl(UAssetLoader* self)
    {
        var loadedObjects = new TArrayList<nint>((UE.Toolkit.Core.Types.Unreal.UE5_4_4.TArray<nint>*)(&self->ObjectReferences),
            _context._toolkitMemory);
        foreach (var loadedObject in loadedObjects)
        {
            if (MemoryToNotify.TryGetValue(*loadedObject.Value, out var memoryNotification))
            {
                var pLoadedObject = *loadedObject.Value;
                if (*(UObject**)pLoadedObject != null)
                {
                    Log.Debug($"[UAssetLoader::CheckStreamedAssets] Loaded file \"{memoryNotification.fileName}\" into 0x{pLoadedObject:X}");
                    memoryNotification.onLoadCb(pLoadedObject);
                } else
                    Log.Warning($"[UAssetLoader::CheckStreamedAssets] ERROR: File \"{memoryNotification.fileName}\" could not be found. The file is likely missing from your mod.");
                MemoryToNotify.Remove(*loadedObject.Value);
            }
        }
    }

    private unsafe void LoadAssetInner(UAssetLoader* loader, string path, nint target, Action<nint> onLoadedCb)
    {
        var assetNameFString = _context._toolkitStrings.CreateFString(path);
        MemoryToNotify.Add(target, (onLoadedCb, path));
        _loadTargetAsset.Wrapper(loader, (FString*)assetNameFString, target);
        _context._toolkitMemory.Free((nint)assetNameFString);
    }

    public unsafe void LoadAsset(IntPtr loader, string path, IntPtr target, Action<IntPtr> onLoaderCb)
        => LoadAssetInner((UAssetLoader*)loader, path, target, onLoaderCb);
}