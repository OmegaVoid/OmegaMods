using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using BackpackOverhaul.BackpackBase;
using InsanityLib;
using InsanityLib.Extensions;
using InsanityLib.Generators.Attributes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackSystem
{
    public class CollectibleBehaviorHeldBackpackBase : CollectibleBehaviorHeldBag, IHeldBag, IAttachedListener
    {
        public CollectibleBehaviorHeldBackpackBase(CollectibleObject collObj) : base(collObj) { }
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
            slot.Inventory.MarkSlotDirty(slot.BagIndex);
            if (slot.Inventory.GetType() == typeof(InventoryGeneric))
            {
                string[] posStr = slot.Inventory.InventoryID.Split("-")[3].Split(",");
                BlockPos bePos = new BlockPos(int.Parse(posStr[0]), int.Parse(posStr[1]), int.Parse(posStr[2]));
                if ((collObj as ItemBackpackBase)!.Api.World.BlockAccessor.GetBlockEntity(bePos) is BlockEntityGroundStorage beGroundStorage)
                {   
                    beGroundStorage.MarkDirty(true);
                }
            }
        }
    }
    public class CollectibleBehaviorHeldBackpackAttachment: CollectibleBehaviorHeldBag, IHeldBag, IAttachedListener
    {
        public CollectibleBehaviorHeldBackpackAttachment(CollectibleObject collObj) : base(collObj) { }
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>();
        }
    }
}
