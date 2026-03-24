using System.Diagnostics;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using p3rpc.asyncassetloader.Template;
using p3rpc.asyncassetloader.Configuration;
using p3rpc.asyncassetloader.Interfaces;
using p3rpc.commonmodutils;
using Reloaded.Memory;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using RyoTune.Reloaded;
using SharedScans.Interfaces;
using UE.Toolkit.Core.Types.Unreal.Factories;
using UE.Toolkit.Interfaces;

namespace p3rpc.asyncassetloader;

public class Mod : ModBase, IExports 
{
    private readonly IModLoader _modLoader;
    private readonly IReloadedHooks? _hooks;
    private readonly ILogger _logger;
    private readonly IMod _owner;
    private Config _configuration;
    private readonly IModConfig _modConfig;
    
    private readonly AssetContext _context;
    private readonly ModuleRuntime<AssetContext> _runtime;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;
        
#if DEBUG
        //Debugger.Launch();
#endif
        
        var process = Process.GetCurrentProcess();
        if (process?.MainModule == null) throw new Exception($"[{_modConfig.ModName}] Process is null");
        var baseAddress = process.MainModule.BaseAddress;
        if (_hooks == null) throw new Exception($"[{_modConfig.ModName}] Could not get controller for Reloaded hooks");
        var logColor = System.Drawing.Color.MediumVioletRed;
        Project.Initialize(_modConfig, _modLoader, _logger, logColor, true);
        Log.LogLevel = _configuration.LogLevel;
        var startupScanner = Utils.GetDependency<IStartupScanner>(_modLoader, _modConfig.ModName, "Reloaded Startup Scanner");
        var utils = Utils.Create(_modLoader, startupScanner, _logger, _hooks, baseAddress, _modConfig.ModName, logColor);
        
        var sharedScans = utils.GetDependencyEx<ISharedScans>("Shared Scans");
        var toolkitObjects = utils.GetDependencyEx<IUnrealObjects>("Object Interface (UE Toolkit)");
        var toolkitMemory = utils.GetDependencyEx<IUnrealMemory>("Memory Interface (UE Toolkit)");
        var toolkitStrings = utils.GetDependencyEx<IUnrealStrings>("String Interface (UE Toolkit)");
        var toolkitClasses = utils.GetDependencyEx<IUnrealClasses>("Classes Interface (UE Toolkit)");
        // var toolkitFactory = utils.GetDependencyEx<IUnrealFactory>("Factory Interface (UE Toolkit)");
        
        _context = new(
            baseAddress, _configuration, _logger, startupScanner, _hooks,
            _modLoader.GetDirectoryForModId(_modConfig.ModId), utils,
            new Memory(), sharedScans, toolkitStrings, toolkitObjects, 
            toolkitMemory, toolkitClasses);
        _runtime = new(_context);
        _runtime.AddModule<AssetLoader>();
        _runtime.RegisterModules();
        _modLoader.AddOrReplaceController<IAssetLoader>(_owner, _runtime.TryGetModule<AssetLoader>()!);
        _modLoader.OnModLoaderInitialized += OnLoaderInit;
        _modLoader.ModLoading += OnModLoading;
    }
    private void OnLoaderInit()
    {
        _modLoader.OnModLoaderInitialized -= OnLoaderInit;
        _modLoader.ModLoading -= OnModLoading;
    }

    private void OnModLoading(IModV1 mod, IModConfigV1 conf) {}

    #region Standard Overrides

    public override void ConfigurationUpdated(Config configuration)
    {
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }

    #endregion

    #region For Exports, Serialization etc.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod()
    {
    }
#pragma warning restore CS8618

    #endregion

    public Type[] GetTypes() => [typeof(IAssetLoader)];
}