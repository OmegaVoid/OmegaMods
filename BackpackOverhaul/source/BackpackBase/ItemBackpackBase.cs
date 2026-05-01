using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Common;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase: Item, IWearableShapeSupplier
    {
        private Shape? shape;
        private string? shapePath;
        public override void OnLoaded(ICoreAPI api)
        {
            shapePath = IAttachableToEntity.FromAttributes(this)?.GetAttachedShape(new ItemStack(), "backpack")?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            base.OnLoaded(api);
        }
        public Shape? GetShape(ItemStack itemStack, Entity targetEntity, string texturePrefixCode)
        {
            string[] p = ["W","E","top"];
            int i = 0;
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (api as ICoreClientAPI)?.ItemTextureAtlas;
            shape = Vintagestory.API.Common.Shape.TryGet(api, shapePath);
            ItemSlot[]? t = (api.World.AllPlayers.First((item) => item.Entity == targetEntity).InventoryManager.Inventories.First((item) => item.Value.ClassName == "backpack").Value as InventoryPlayerBackpacks)?.bagSlots.Where((item) => item.Itemstack != null).Where((item) => item.Itemstack!.Item != this).ToArray();
            string? childPath = null;
            if (shape != null & targetAtlas != null & shapePath != null & t != null)
            {
                foreach (ItemSlot slot in t!)
                {
                    childPath = slot.Itemstack!.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, slot.Itemstack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
                    if (Vintagestory.API.Common.Shape.TryGet(api,childPath) is Shape childShape)
                    {
                        childShape.Elements[0].StepParentName = p[i];
                        shape!.StepParentShape(childShape, texturePrefixCode, childPath, shapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        i += 1;
                    }
                }
            }
            return shape;
        }
    }
}
