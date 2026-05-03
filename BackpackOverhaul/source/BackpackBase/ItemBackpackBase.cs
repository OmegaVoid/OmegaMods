using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Common;
using Vintagestory.GameContent;
using static System.Net.Mime.MediaTypeNames;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase: Item, IWearableShapeSupplier
    {
        public Shape? _shape;
        private Shape? shape;
        private string? shapePath;
        private string? baseShapePath;
        private Shape? baseShape;
        public ShapeTextureSource? texSource;
        public override void OnLoaded(ICoreAPI api)
        {
            baseShapePath = Shape.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            baseShape = Vintagestory.API.Common.Shape.TryGet(api, baseShapePath);
            shapePath = IAttachableToEntity.FromAttributes(this)?.GetAttachedShape(new ItemStack(), "backpack")?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            base.OnLoaded(api);
        }
        public Shape? GetShape(ItemStack itemStack, Entity targetEntity, string texturePrefixCode)
        {
            string[] p = ["W","E","top"];
            int i = 0;
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (api as ICoreClientAPI)?.ItemTextureAtlas;
            ITextureAtlasAPI? basetargetAtlas = (api as ICoreClientAPI)?.BlockTextureAtlas;
            shape = Vintagestory.API.Common.Shape.TryGet(api, shapePath);
            ItemSlot[]? t = (api.World.AllPlayers.First((item) => item.Entity == targetEntity).InventoryManager.Inventories.First((item) => item.Value.ClassName == "backpack").Value as InventoryPlayerBackpacks)?.bagSlots.Where((item) => item.Itemstack != null).Where((item) => item.Itemstack!.Item != this).ToArray();
            string childPath = "";
            if (shape != null & targetAtlas != null & shapePath != null & t != null)
                _shape = baseShape!.Clone();
                foreach (ItemSlot slot in t!)
                    {
                    childPath = slot.Itemstack!.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, slot.Itemstack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(api,childPath) is Shape childShape)
                    {
                        childShape.Elements[0].StepParentName = p[i];                    
                        shape!.StepParentShape(childShape, texturePrefixCode, childPath, shapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        _shape!.StepParentShape(childShape, texturePrefixCode, childPath, shapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        texSource = new ShapeTextureSource((api as ICoreClientAPI)!, _shape, "mylog", collectedTextures, (p) => p);
                        i += 1;
                    }
                }
            return shape;
        }
    }
}
