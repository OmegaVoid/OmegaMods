using InsanityLib.Generators.Attributes;

namespace BackpackOverhaul;

public class BackpackOverhaulConfig
{
    [AutoConfig("BackpackOverhaulConfig.json", ServerSync = true)]
    public static BackpackOverhaulConfig Instance { get; set; } = null!;
}