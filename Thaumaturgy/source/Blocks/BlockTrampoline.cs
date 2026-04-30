using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Thaumaturgy.Blocks;

public class BlockTrampoline : Block
{
    //Any code within this 'override' function will be called when a trampoline block is placed. 
    public override void OnBlockPlaced(IWorldAccessor world, BlockPos blockPos, ItemStack byItemStack = null)
    {
        //Log a message to the console.
        api.Logger.Event("Trampoline Block Placed!");
        //Perform any default logic when our block is placed.
        base.OnBlockPlaced(world, blockPos, byItemStack);
    }

    //Any code within this 'override' function will be called when a trampoline block is broken.
    public override void OnBlockBroken(IWorldAccessor world, BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1)
    {
        //Log a message to the console.
        api.Logger.Event("Trampoline Block Broken!");
        //Perform any default logic when our block is broken (e.g., dropping the block as an item.)
        base.OnBlockBroken(world, pos, byPlayer, dropQuantityMultiplier);
    }
}