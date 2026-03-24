using p3rpc.commonmodutils;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SharedScans.Interfaces;
using UE.Toolkit.Interfaces;

namespace p3rpc.asyncassetloader;

public class AssetContext : UnrealToolkitContext
{
    public AssetContext(long baseAddress, IConfigurable config, ILogger logger, IStartupScanner startupScanner, 
        IReloadedHooks hooks, string modLocation, Utils utils, Memory memory, ISharedScans sharedScans, 
        IUnrealStrings toolkitStrings, IUnrealObjects toolkitObjects, IUnrealMemory toolkitMemory, 
        IUnrealClasses toolkitClasses) : base(baseAddress, config, logger, startupScanner, hooks, modLocation, utils, 
        memory, sharedScans, toolkitStrings, toolkitObjects, toolkitMemory, toolkitClasses)
    {
    }
}