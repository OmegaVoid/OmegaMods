using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using BackpackOverhaul.BackpackBase;
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
        public CollectibleBehaviorHeldBackpackBase(CollectibleObject collObj) : base(collObj) { }
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            (bagStack.Item as ItemBackpackBase)?.RefreshShape(bagStack);
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>(); //,properties: {tags: ["tool-knife"]}
        }
        public override EnumItemStorageFlags GetStorageFlags(ItemStack bagstack)
        {
            (bagstack.Item as ItemBackpackBase)?.RefreshShape(bagstack);
            return EnumItemStorageFlags.Backpack;
        }
    }
}
