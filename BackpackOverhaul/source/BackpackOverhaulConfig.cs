using InsanityLib.Generators.Attributes;
using JetBrains.Annotations;

namespace BackpackOverhaul;

public class BackpackOverhaulConfig
{
    [AutoConfig("BackpackOverhaulConfig.json", ServerSync = true)]
    public static BackpackOverhaulConfig Instance { [UsedImplicitly] get; set; } = null!;
}