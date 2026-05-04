using BackpackOverhaul.BackpackSystem;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Common;
using Vintagestory.GameContent;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase: Item, IWearableShapeSupplier
    {
        public Shape? shape { get; private set; }
        private Shape? attachedShape;
        private string? attachedShapePath;
        private string? baseShapePath;
        private Shape? baseShape;
        public ShapeTextureSource? texSource { get; private set; }
        public override void OnLoaded(ICoreAPI api)
        {
            baseShapePath = Shape.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            baseShape = Vintagestory.API.Common.Shape.TryGet(api, baseShapePath);
            attachedShapePath = IAttachableToEntity.FromAttributes(this)?.GetAttachedShape(new ItemStack(), "backpack")?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            attachedShape = Vintagestory.API.Common.Shape.TryGet(api, attachedShapePath);
            base.OnLoaded(api);
        }
        public Shape? GetShape(ItemStack itemStack, Entity targetEntity, string texturePrefixCode)
        {
            string[] p = ["W","E","top"];
            int i = 0;
            Shape? _attachedShape = attachedShape?.Clone();
            Shape? _shape = baseShape!.Clone();
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (api as ICoreClientAPI)?.ItemTextureAtlas;
            //ItemSlot[]? slots = (api.World.AllPlayers.First((item) => item.Entity == targetEntity).InventoryManager.Inventories.First((item) => item.Value.ClassName == "backpack").Value as InventoryPlayerBackpacks)?.bagSlots.Where((item) => item.Itemstack != null).Where((item) => item.Itemstack!.Item != this).ToArray();
            ItemStack[]? stacks = this.GetCollectibleBehavior<CollectibleBehaviorHeldBackpackBase>(false).GetContents(itemStack, api.World);
            //if (_attachedShape != null & targetAtlas != null & slots != null)
            if (_attachedShape != null & targetAtlas != null & stacks != null)
                //shape = baseShape!.Clone();
                //foreach (ItemSlot slot in slots!)
                foreach (ItemStack? stack in stacks!)
                {
                    //string childPath = slot.Itemstack!.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, slot.Itemstack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    string childPath = stack?.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, stack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(api,childPath) is Shape childShape)
                    {
                        childShape.Elements[0].StepParentName = p[i];                    
                        _attachedShape!.StepParentShape(childShape, texturePrefixCode, childPath, attachedShapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        _shape!.StepParentShape(childShape, texturePrefixCode, childPath, attachedShapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        texSource = new ShapeTextureSource((api as ICoreClientAPI)!, _shape, "mylog", collectedTextures, (p) => p);
                        i += 1;
                    }
                }
            shape = _shape;
            return _attachedShape;
        }
    }
}
