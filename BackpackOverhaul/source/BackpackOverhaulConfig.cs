using InsanityLib.Generators.Attributes;

namespace BackpackOverhaul;

/// <summary>
/// 
/// </summary>
public class BackpackOverhaulConfig
{
    /// <summary>
    /// Singleton Instance
    /// </summary>
    [AutoConfig("BackpackOverhaulConfig.json", ServerSync = true)]
    public static BackpackOverhaulConfig Instance { get; set; } = null!;
}