using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;

namespace Thaumaturgy;

[SuppressMessage("Usage", "CA2263:Prefer generic overload when type is known")]
public static class Extensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "GetNameToCodeMapping")]
    private static extern Dictionary<string, HashSet<string>> GetNameToCodeMapping(RecipeBase recipeBase, IWorldAccessor world);
    
    extension(RecipeBase recipeBase)
    {
        public Dictionary<string, HashSet<string>> GetNameToCodeMappingPublic(IWorldAccessor world) =>
            GetNameToCodeMapping(recipeBase, world);
    }
    
}