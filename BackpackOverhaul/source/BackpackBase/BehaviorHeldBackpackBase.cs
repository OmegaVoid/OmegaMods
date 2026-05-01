using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using InsanityLib;
using InsanityLib.Generators.Attributes;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackSystem
{
    public class CollectibleBehaviorHeldBackpackBase : CollectibleBehaviorHeldBag, IHeldBag, IAttachedListener
    {
        public CollectibleBehaviorHeldBackpackBase(CollectibleObject collObj) : base(collObj)
        {
            EntityBehaviorPlayerInventory? Test;
        }
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>();
        }
        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);
        }
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handHandling, ref EnumHandling handling)
        {
            EntityPlayer entityPlayer = byEntity as EntityPlayer;
            EntityBehaviorPlayerInventory inv = entityPlayer.GetBehavior<EntityBehaviorPlayerInventory>();
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handHandling, ref handling);
        }        
    }
}
