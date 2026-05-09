using Newtonsoft.Json.Linq;
using Thaumaturgy.Recipes;
using VintageEngineering.RecipeSystem.Recipes;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace Thaumaturgy.RecipeSystem;

public class ThaumaturgyRecipeLoader : ModSystem
{
    private ICoreServerAPI? _sapi;
    private bool _classExclusiveRecipes = true;
    
    public override double ExecuteOrder()
        {
            return 1;
        }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void AssetsLoaded(ICoreAPI api)
        {
            _sapi = api as ICoreServerAPI;
            if (_sapi == null) return;
            _classExclusiveRecipes = _sapi.World.Config.GetBool("classExclusiveRecipes", true);

            var rs = _sapi.ModLoader.GetModSystem<ThaumaturgyRecipeRegistrySystem>(true);

            if (rs == null) 
            {
                throw new InvalidOperationException("Thaumaturgy | RecipeLoader: Error retrieving ThaumaturgyRecipeRegistrySystem! Cannot register Thaumaturgy Recipes!");
            }

            // Now for all the recipe loading...
            LoadRecipes<InfusionRecipe>("ve metal press recipe", "recipes/vemetalpress", rs.RegisterInfusionRecipe);
            _sapi.World.Logger.StoryEvent(Lang.Get("thaumaturgy:storyevent-recipes"));
        }

        public void LoadRecipes<T>(string name, string path, Action<T> RegisterMethod) where T : ThaumaturgyRecipeBase<T>, new()
        {
            Dictionary<AssetLocation, JToken> many = _sapi?.Assets.GetMany<JToken>(_sapi?.Server.Logger, path, null) ?? [];
            var recipeQuantity = 0;
            var quantityRegistered = 0;
            var quantityIgnored = 0;
            foreach (var val in many)
            {
                switch (val.Value)
                {
                    case JObject:
                        LoadGenericRecipe<T>(name, val.Key, val.Value.ToObject<T>(val.Key.Domain, null), RegisterMethod, ref quantityRegistered, ref quantityIgnored);
                        recipeQuantity++;
                        break;
                    case JArray array:
                        foreach (var token in array)
                        {
                            LoadGenericRecipe<T>(name, val.Key, token.ToObject<T>(val.Key.Domain, null), RegisterMethod, ref quantityRegistered, ref quantityIgnored);
                            recipeQuantity++;
                        }

                        break;
                }
            }
            _sapi?.World.Logger.Event($"{quantityRegistered} {name}s loaded {(quantityIgnored > 0 ? $" {quantityIgnored} could not be resolved" : "")}");
        }
        private void LoadGenericRecipe<T>(string className, AssetLocation path, T recipe, Action<T> RegisterMethod, ref int quantityRegistered, ref int quantityIgnored) where T : ThaumaturgyRecipeBase<T>, new()
        {
            if (_sapi is null) return;
            
            if (!recipe.Enabled)
                return;
            if (recipe.Name == null) recipe.Name = path;
            ref var ptr = ref recipe;
            var nameToCodeMapping = ptr.GetNameToCodeMappingPublic(_sapi.World);
            if (nameToCodeMapping.Count > 0)
            {
                var subRecipes = new List<T>();
                var qCombs = 0;
                var first = true;
                foreach (var val2 in nameToCodeMapping)
                {
                    if (first)
                        qCombs = val2.Value.Length;
                    else
                        qCombs *= val2.Value.Length;
                    first = false;
                }
                first = true;
                foreach (var (variantCode, variants) in nameToCodeMapping)
                {
                    for (var i = 0; i < qCombs; i++)
                    {
                        T rec;
                        if (first)
                            subRecipes.Add(rec = recipe.Clone());
                        else
                            rec = subRecipes[i];
                        if (rec.Ingredients != null)
                            foreach (var ingred in rec.Ingredients)
                                if (ingred.Name == variantCode)
                                    ingred.Code = ingred.Code?.CopyWithPath(ingred.Code.Path.Replace("*", variants[i % variants.Length]));
                        if (rec.Outputs == null) continue;
                        foreach (var output in rec.Outputs)
                            output.FillPlaceHolder(variantCode, variants[i % variants.Length]);
                    }
                    first = false;
                }
                if (subRecipes.Count == 0) _sapi?.World.Logger.Warning($"VintEng: {path} file {className} make uses of wildcards, but no blocks or item matching those wildcards were found.");
                using var enumerator2 = subRecipes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    var subRecipe = enumerator2.Current;
                    ref var ptr2 = ref subRecipe;
                    t = default;
                    if (t == null)
                    {
                        t = subRecipe;
                        ptr2 = ref t!;
                    }

                    if (!ptr2.Resolve(_sapi?.World, className + " " + (path != null ? path.ToString() : null)))
                        quantityIgnored++;
                    else
                    {
                        RegisterMethod(subRecipe);
                        quantityRegistered++;
                    }
                }
                return;
            }
            ref var ptr3 = ref recipe;
            t = default;
            if (t == null)
            {
                t = recipe;
                ptr3 = ref t!;
            }
            if (!ptr3.Resolve(_sapi?.World, $"{className} {path}"))
            {
                quantityIgnored++;
                return;
            }
            RegisterMethod(recipe);
            quantityRegistered++;
        }
}