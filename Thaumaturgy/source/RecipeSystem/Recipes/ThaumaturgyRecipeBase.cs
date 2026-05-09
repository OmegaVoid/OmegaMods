using Vintagestory.API;
using Vintagestory.API.Common;

namespace Thaumaturgy.Recipes;

public abstract class ThaumaturgyRecipeBase<T> : RecipeBase, IConcreteCloneable<T> where T : RecipeBase, new()
{
    /// <summary>The resulting stack when the recipe is created.</summary>
    [DocumentAsJson("Required", "", false)]
    public CraftingRecipeIngredient? Output { get; set; }

    [DocumentAsJson("Required", "", false)]
    public Dictionary<string, CraftingRecipeIngredient>? Ingredients { get; set; }

    /// <summary>
    /// Info used by the handbook. By default, all recipes for an object will appear in a single preview. This allows you to split recipe previews into multiple.
    /// </summary>
    [DocumentAsJson("Optional", "0", false)]
    public int RecipeGroup { get; set; }
    
    protected override void CloneTo(object recipe)
    {
        base.CloneTo(recipe);
        if (recipe is not ThaumaturgyRecipeBase<T> thaumaturgyRecipe)
            throw new ArgumentException("CloneTo should take object of same class or it subclass");
        thaumaturgyRecipe.Output = Output?.Clone();
        thaumaturgyRecipe.Ingredients = new Dictionary<string, CraftingRecipeIngredient>();
        if (Ingredients != null)
            foreach (var (key, recipeIngredient) in Ingredients)
                thaumaturgyRecipe.Ingredients[key] = recipeIngredient.Clone();

        thaumaturgyRecipe.RecipeGroup = RecipeGroup;
    }

    /// <summary>Creates a deep copy</summary>
    /// <returns></returns>
    public override T Clone()
    {
        var recipe = new T();
        CloneTo(recipe);
        return recipe;
    }
    
    public override IEnumerable<IRecipeIngredient> RecipeIngredients =>
        Ingredients?.Values ?? throw new InvalidOperationException(
            $"Thaumaturgy recipe '{Name}' has no ingredients specified or ingredients are failed to resolve");

    public override IRecipeOutput RecipeOutput =>
        Output ?? throw new InvalidOperationException($"Thaumaturgy recipe '{Name}' has no output specified");
    
    public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
    {
        if (Output == null)
        {
            world.Logger.Error($"Thaumaturgy Recipe '{Name}' has no output specified.");
            return false;
        }

        if (Ingredients == null)
        {
            world.Logger.Error($"Thaumaturgy Recipe with output '{Output.Code}' has no ingredients.");
            return false;
        }

        return true;
    }
    
}