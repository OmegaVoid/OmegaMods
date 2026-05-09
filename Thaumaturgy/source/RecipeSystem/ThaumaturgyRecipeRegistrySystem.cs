using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Thaumaturgy.Recipes;
using VintageEngineering.RecipeSystem;
using VintageEngineering.RecipeSystem.Recipes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace Thaumaturgy.RecipeSystem;

public class ThaumaturgyRecipeRegistrySystem : ModSystem
{
    public static bool CanRegister;

    public List<InfusionRecipe> InfusionRecipes = [];

    public override double ExecuteOrder() => 0.6;

    public override void StartPre(ICoreAPI api) => CanRegister = true;

    public override void AssetsLoaded(ICoreAPI api)
    {
        if (api is not ICoreServerAPI sapi)
            return;
    }


    /// <inheritdoc />
    public override void Start(ICoreAPI api)
    {
        InfusionRecipes = api.RegisterRecipeRegistry<RecipeRegistryGeneric<InfusionRecipe>>("infusionrecipes").Recipes;
    }

    public void RegisterInfusionRecipe(InfusionRecipe infusionRecipe)
    {
        if (!CanRegister)
            throw new InvalidOperationException(
                "Thaumaturgy | RecipeRegistrySystem: Can no longer register Infusion recipes. Register during AssetsLoaded/AssetsFinalize and with ExecuteOrder < 99999");

        infusionRecipe.RecipeId = InfusionRecipes.Count + 1;
        InfusionRecipes.Add(infusionRecipe);
    }


    /// <summary>
    /// Finds all recipe outputs that take the ingredient, ignoring stack attributes.
    /// </summary>
    /// <param name="input">the input ingredient to search for</param>
    /// <returns>A dictionary of recipe outputs. The outputs are grouped in the dictionary their recipe name.</returns>
    private static IReadOnlyDictionary<AssetLocation, List<ItemStack>>
        GetOutputsForIngredient<T>(IReadOnlyList<IVEMachineRecipeBase<T>> recipes, ItemStack input)
    {
        Dictionary<AssetLocation, List<ItemStack>?>? result = null;
        foreach (var recipe in recipes)
            for (var inputIndex = 0; inputIndex < recipe.Ingredients.Length; ++inputIndex)
            {
                if (!recipe.SatisfiesAsIngredient(inputIndex, input, false)) continue;
                for (var outputIndex = 0; outputIndex < recipe.Outputs.Length; ++outputIndex)
                {
                    result ??= [];
                    if (!result.TryGetValue(recipe.Name,
                            out var resultList))
                    {
                        resultList = [];
                        result.Add(recipe.Name, resultList);
                    }

                    resultList?.Add(recipe.GetResolvedOutput(outputIndex));
                }
                break;
            }

        return result as IReadOnlyDictionary<AssetLocation, List<ItemStack>> ??
               ImmutableDictionary<AssetLocation, List<ItemStack>>.Empty;
    }

    /// <summary>
    /// Finds the ingredients for any recipes that produces the output, ignoring stack attributes.
    /// </summary>
    /// <param name="output">the recipe output to search for</param>
    /// <param name="allStacks">every resolved item</param>
    /// <returns>A dictionary of recipe ingredients. The ingredients are grouped in the dictionary their recipe name.</returns>
    private static IReadOnlyDictionary<AssetLocation, List<ItemStack>>
        GetIngredientsForOutput<T>(ICoreClientAPI capi, IReadOnlyList<IVEMachineRecipeBase<T>> recipes,
            ItemStack output, ItemStack[] allStacks)
    {
        Dictionary<AssetLocation, List<ItemStack>?>? result = null;
        foreach (var recipe in recipes)
            for (var outputIndex = 0; outputIndex < recipe.Outputs.Length; ++outputIndex)
            {
                if (!(recipe.GetResolvedOutput(outputIndex)?.Equals(
                          capi.World, output, GlobalConstants.IgnoredStackAttributes) ??
                      false)) continue; // the null ref was on this line
                for (var inputIndex = 0; inputIndex < recipe.Ingredients.Length; ++inputIndex)
                {
                    result ??= new Dictionary<AssetLocation, List<ItemStack>?>();
                    if (!result.TryGetValue(recipe.Name,
                            out var resultList))
                    {
                        resultList = [];
                        result.Add(recipe.Name, resultList);
                    }

                    var resolved = recipe.GetResolvedInput(inputIndex);
                    if (resolved != null)
                        resultList?.Add(recipe.GetResolvedInput(inputIndex));
                    else
                        resultList?.AddRange(allStacks.Where(item =>
                            recipe.SatisfiesAsIngredient(inputIndex, item, false)));
                }

                break;
            }

        return result as IReadOnlyDictionary<AssetLocation, List<ItemStack>> ??
               ImmutableDictionary<AssetLocation, List<ItemStack>>.Empty;
    }

    private static void AddRecipeProcessesInto<T>(ICoreClientAPI capi, IReadOnlyList<IVEMachineRecipeBase<T>> recipes,
        string processesIntoVerb,
        ActionConsumable<string> openDetailPageFor, ItemStack stack,
        List<RichTextComponentBase> components, ref bool haveText)
    {
        var groupedOutputs = GetOutputsForIngredient(recipes, stack);
        if (groupedOutputs.Count == 0)
            return;

        CollectibleBehaviorHandbookTextAndExtraInfoPatch.AddHeading(components, capi, processesIntoVerb, ref haveText);

        components.AddRange(groupedOutputs.Values.Select(group =>
            new SlideshowItemstackTextComponent(capi, group.ToArray(), GuiStyle.LargeFontSize, EnumFloat.Inline,
                    (ingredient) => openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(ingredient)))
                { ShowStackSize = true }).Cast<RichTextComponentBase>());
        ;
        // Add a newline
        components.Add(new ClearFloatTextComponent(capi,
            CollectibleBehaviorHandbookTextAndExtraInfoPatch.MarginBottom));
    }

    private void AddRecipeCreatedBy<T>(ICoreClientAPI capi, IReadOnlyList<IVEMachineRecipeBase<T>> recipes,
        string requiredMachine, string createdByVerb,
        ItemStack[] allStacks, ActionConsumable<string> openDetailPageFor,
        ItemStack stack, ref List<RichTextComponentBase> components)
    {
        var groupedInputs = GetIngredientsForOutput(capi, recipes, stack, allStacks);
        if (groupedInputs.Count == 0)
            return;
        if (components == null)
            components = [];
        else
            components.Add(new ClearFloatTextComponent(capi,
                CollectibleBehaviorHandbookTextAndExtraInfoPatch.SmallPadding));
        CollectibleBehaviorHandbookTextAndExtraInfoPatch.AddSubHeading(components, capi, openDetailPageFor,
            createdByVerb, null);

        var first = true;
        foreach (var group in groupedInputs.Values)
        {
            if (!first)
            {
                // Add a newline
                components.Add(new ClearFloatTextComponent(capi,
                    CollectibleBehaviorHandbookTextAndExtraInfoPatch.SmallPadding));
            }

            first = false;
            SlideshowItemstackTextComponent input =
                new(capi, group.ToArray(),
                        GuiStyle.LargeFontSize, EnumFloat.Inline,
                        (ingredient) =>
                            openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(
                                ingredient)))
                    { ShowStackSize = true };
            components.Add(input);
            if (requiredMachine == null) continue;
            {
                RichTextComponent text = new(capi, Lang.Get("vinteng:in machine"), CairoFont.WhiteSmallText())
                {
                    VerticalAlign = EnumVerticalAlign.Middle
                };
                components.Add(text);
                var machineItems = Array.Empty<ItemStack>();
                if (recipeMachines.TryGetValue(requiredMachine, out List<Block> machineBlocks))
                {
                    machineItems = machineBlocks.Select((block) => new ItemStack(block)).ToArray();
                }

                SlideshowItemstackTextComponent machines =
                    new(capi, machineItems,
                            GuiStyle.LargeFontSize, EnumFloat.Inline,
                            (ingredient) =>
                                openDetailPageFor(GuiHandbookItemStackPage.PageCodeForStack(
                                    ingredient)))
                        { ShowStackSize = true };
                components.Add(machines);
            }
        }

        ;
        // Add a newline
        components.Add(new ClearFloatTextComponent(capi,
            CollectibleBehaviorHandbookTextAndExtraInfoPatch.MarginBottom));
    }

    private void AddRecipesToHandbook<T>(ICoreAPI api, IReadOnlyList<IVEMachineRecipeBase<T>> recipes,
        string requiredMachine, string processesIntoVerb, string createdByVerb)
    {
        if (api.Side != EnumAppSide.Client)
        {
            return;
        }

        CollectibleBehaviorHandbookTextAndExtraInfoPatch.ProcessesInto +=
            delegate(ICoreClientAPI capi, ActionConsumable<string> openDetailPageFor, ItemStack stack,
                List<RichTextComponentBase> components, ref bool haveText)
            {
                AddRecipeProcessesInto(capi, recipes, processesIntoVerb, openDetailPageFor, stack, components,
                    ref haveText);
            };

        CollectibleBehaviorHandbookTextAndExtraInfoPatch.CreatedBy +=
            delegate(ICoreClientAPI capi, ItemStack[] allStacks, ActionConsumable<string> openDetailPageFor,
                ItemStack stack, ref List<RichTextComponentBase> components)
            {
                AddRecipeCreatedBy(capi, recipes, requiredMachine, createdByVerb,
                    allStacks, openDetailPageFor, stack, ref components);
            };
    }
}