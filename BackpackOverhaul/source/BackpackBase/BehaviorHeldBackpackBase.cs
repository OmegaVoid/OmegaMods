using System;
using System.Collections.Generic;
using System.Text;
using InsanityLib;
using InsanityLib.Generators.Attributes;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackSystem
{
    /// <inheritdoc />
    public class CollectibleBehaviorHeldBackpackBase : CollectibleBehaviorHeldBag, IHeldBag, IAttachedListener
    {
        /// <inheritdoc />
        public CollectibleBehaviorHeldBackpackBase(CollectibleObject collObj) : base(collObj)
        {
        }
        /// <inheritdoc />
        public override TagSet GetStorageTags(ItemStack bagStack)
        {
            return bagStack.ItemAttributes["backpack"]["storageTags"].AsObject<TagSet>();
        }
    }
}
