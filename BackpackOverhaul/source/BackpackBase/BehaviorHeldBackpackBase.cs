using PlayerInventoryLib;
using PlayerInventoryLib.Backpacks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase
{
    public class CollectibleBehaviorHeldBackpackBase(CollectibleObject collObj) : CollectibleBehaviorHeldBag(collObj), IHeldBag
    {
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            (bagStack.Item as ItemBackpackBase)?.RefreshShape(bagStack);
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>();
        }
        public override EnumItemStorageFlags GetStorageFlags(ItemStack bagstack)
        {
            (bagstack.Item as ItemBackpackBase)?.RefreshShape(bagstack);
            return EnumItemStorageFlags.Backpack;
        }
        public new void Store(ItemStack bagstack, ItemSlotBagContent slot)
        {
            base.Store(bagstack, slot);
            if (slot.Itemstack?.Item.Code.Path == "testbag")
            {
                var bagBehavior = slot.Itemstack.Item.GetBehavior<CollectibleBehaviorHeldBackpackAttachment>();
                bagBehavior?.GetOrCreateSlots(slot.Itemstack, slot.Inventory,1, ((collObj as ItemBackpackBase)!.Api.World));
            }
            slot.Inventory.MarkSlotDirty(slot.BagIndex);
            if (slot.Inventory.GetType() != typeof(InventoryGeneric)) return;
            string[] posStr = slot.Inventory.InventoryID.Split("-")[3].Split(",");
            BlockPos bePos = new BlockPos(int.Parse(posStr[0]), int.Parse(posStr[1]), int.Parse(posStr[2]));
            if ((collObj as ItemBackpackBase)!.Api.World.BlockAccessor.GetBlockEntity(bePos) is BlockEntityGroundStorage beGroundStorage)
            {
                beGroundStorage.MarkDirty(true);
            }
        }

        public new List<ItemSlotBagContent> GetOrCreateSlots(ItemStack bagstack, InventoryBase parentinv, int bagIndex, IWorldAccessor world)
        {
            return base.GetOrCreateSlots(bagstack, parentinv, bagIndex, world);
        }
    }
    public class CollectibleBehaviorHeldBackpackAttachment(CollectibleObject collObj) : CollectibleBehaviorHeldBag(collObj)
    {
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>();
        }
    }
}
