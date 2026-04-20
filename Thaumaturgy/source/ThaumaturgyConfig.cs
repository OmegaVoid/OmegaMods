using InsanityLib.Generators.Attributes;

namespace Thaumaturgy;

/// <summary>
/// 
/// </summary>
public class ThaumaturgyConfig
{
    
    /// <summary>
    /// Singleton Instance
    /// </summary>
    [AutoConfig("ThaumaturgyConfig.json", ServerSync = true)]
    public static ThaumaturgyConfig Instance { get; set; } = null!;
}