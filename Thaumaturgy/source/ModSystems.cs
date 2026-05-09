//using ConfigLib;
// using ImGuiNET;

using InsanityLib.Generators.Attributes;
using Thaumaturgy.Recipes;
using VintageEngineering.RecipeSystem;
using VintageEngineering.RecipeSystem.Recipes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;
using Vintagestory.ServerMods;

//using VSImGui;
// using VSImGui.API;

[assembly:
    AutoRegistryName(RemovePrefix =
    [
        "CollectibleBehavior", "BlockEntity", "BlockEntityBehavior", "BlockBehavior", "Block", "EntityBehavior",
        "Entity", "Item"
    ])]

namespace Thaumaturgy;

/// <inheritdoc />
public partial class ThaumaturgyModSystem
{
    // private ImGuiModSystem _modSystem = null!;

    /// <summary>
    /// 
    /// </summary>
    public readonly ThaumaturgyConfig Settings = new();

    /// <inheritdoc />
    public override void StartPre(ICoreAPI api)
    {
        base.StartPre(api);
        AutoSetup(api);
    }

    /// <inheritdoc />
    public override void Start(ICoreAPI api)
    {
    }
    

    /// <inheritdoc />
    public override void StartClientSide(ICoreClientAPI api)
    {
        // _modSystem = api.ModLoader.GetModSystem<ImGuiModSystem>();
        // _modSystem.Draw += Draw;
    }

    /// <inheritdoc />
    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);
        AutoAssetsLoaded(api);
        if (api is not ICoreServerAPI serverApi)
            return;
        RecipeLoader.LoadRecipes<InfusionRecipe>(serverApi, "infusion", "recipes/grid", classExclusiveRecipes,
            r => serverApi.RegisterRecipeRegistry<RecipeRegistryGeneric<InfusionRecipe>>().RegisterCraftingRecipe(r as InfusionRecipe));
    }
    
    public void RegisterInfusionRecipe(InfusionRecipe recipe)
    {
        if (!RecipeRegistrySystem.canRegister)
            throw new InvalidOperationException("Coding error: Can no long register cooking recipes. Register them during AssetsLoad/AssetsFinalize and with ExecuteOrder < 99999");
        recipe.RecipeId = InfusionRecipes.Count + 1;
        if (recipe.Code == (AssetLocation) null)
            this.Api.Logger.Warning("Smithing recipe with output {0} has no code. For 1.23 all smithing recipes need to have a unique, unchanging code", (object) recipe.Output.Code);
        else if (InfusionRecipes.FirstOrDefault<SmithingRecipe>((System.Func<SmithingRecipe, bool>) (r => r.Code == recipe.Code)) != null)
            this.api.Logger.Warning("Smithing recipe with output {0} has a code that is already used by another recipe. For 1.23 all smithing recipes need to have a unique, unchanging code", (object) recipe.Output.Code);
        InfusionRecipes.Add(recipe);
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        AutoDispose();
        base.Dispose();
    }

    // private CallbackGUIStatus Draw(float deltaSeconds)
    // {
    //     ImGui.Begin("ImGui example");
    //
    //     var roll = 0f;
    //     ImGui.SliderFloat("Roll", ref roll, -90, 90);
    //
    //     ImGui.End();
    //     return CallbackGUIStatus.GrabMouse;
    // }
}