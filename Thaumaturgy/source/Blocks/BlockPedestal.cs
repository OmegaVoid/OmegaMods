using Thaumaturgy.BlockEntity;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace Thaumaturgy.Blocks;

public class BlockPedestal : Block
{
    //Any code within this 'override' function will be called when a trampoline block is placed. 
    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
    {
        //Log a message to the console.
        api.Logger.Event("Pedestal Block Placed!");
        //Perform any default logic when our block is placed.
        base.OnBlockPlaced(world, blockPos, byItemStack);
    }

    //Any code within this 'override' function will be called when a trampoline block is broken.
    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer,
        float dropQuantityMultiplier = 1)
    {
        //Log a message to the console.
        api.Logger.Event("Pedestal Block Broken!");
        //Perform any default logic when our block is broken (e.g., dropping the block as an item.)
        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
    }

    private WorldInteraction[] _interactions = null!;

    public override void OnLoaded(ICoreAPI _api)
    {
        base.OnLoaded(_api);
        if (_api.Side != EnumAppSide.Client)
            return;
        _interactions = ObjectCacheUtil.GetOrCreate(_api, "pedestalInteractions", () => new WorldInteraction[]
        {
            new()
            {
                MouseButton = EnumMouseButton.Right,
                ActionLangCode = "blockhelp-pedestal-place"
            },
            new()
            {
                MouseButton = EnumMouseButton.Right,
                RequireFreeHand = true,
                ActionLangCode = "blockhelp-pedestal-remove"
            }
        });
    }

    public override bool DoPartialSelection(IWorldAccessor world, BlockPos pos) => true;

    public override bool OnBlockInteractStart(
        IWorldAccessor world,
        IPlayer byPlayer,
        BlockSelection blockSel) =>
        world.BlockAccessor.GetBlockEntity(blockSel.Position) is BlockEntityPedestal blockEntity
            ? blockEntity.OnInteract(byPlayer, blockSel)
            : base.OnBlockInteractStart(world, byPlayer, blockSel);

    public override WorldInteraction[] GetPlacedBlockInteractionHelp(
        IWorldAccessor world,
        BlockSelection selection,
        IPlayer forPlayer) =>
        _interactions.Append<WorldInteraction>(base.GetPlacedBlockInteractionHelp(world, selection, forPlayer));
}