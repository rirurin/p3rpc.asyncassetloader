using System.ComponentModel;
using p3rpc.asyncassetloader.Template.Configuration;
using Reloaded.Mod.Interfaces.Structs;
using RyoTune.Reloaded;

namespace p3rpc.asyncassetloader.Configuration;

public class Config : Configurable<Config>
{
    [DisplayName("Log Level")]
    [Category("Logging")]
    [DefaultValue(LogLevel.Information)]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
}

/// <summary>
/// Allows you to override certain aspects of the configuration creation process (e.g. create multiple configurations).
/// Override elements in <see cref="ConfiguratorMixinBase"/> for finer control.
/// </summary>
public class ConfiguratorMixin : ConfiguratorMixinBase
{
    // 
}