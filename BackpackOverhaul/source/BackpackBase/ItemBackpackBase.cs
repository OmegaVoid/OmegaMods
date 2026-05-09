using PlayerInventoryLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase : Item, IWearableShapeSupplier
    {
        public Shape? Combshape { get; private set; }
        public  ICoreAPI Api => api;
        private Shape? _attachedShape;
        private string? _attachedShapePath;
        private string? _baseShapePath;
        private Shape? _baseShape;
        public ShapeTextureSource? TexSource { get; private set; }
        public override void OnLoaded(ICoreAPI capi)
        {
            _baseShapePath = Shape.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            _baseShape = Vintagestory.API.Common.Shape.TryGet(capi, _baseShapePath);
            _attachedShapePath = IAttachableToEntity.FromAttributes(this)?.GetAttachedShape(new ItemStack(), "backpack")?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json");
            _attachedShape = Vintagestory.API.Common.Shape.TryGet(capi, _attachedShapePath);
            base.OnLoaded(capi);
        }
        public Shape? GetShape(ItemStack itemStack, Entity targetEntity, string texturePrefixCode)
        {
            int i = 1;
            Shape? attachedShape = this._attachedShape?.Clone();
            Shape? shape = _baseShape?.Clone();
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (Api as ICoreClientAPI)?.ItemTextureAtlas;
            List<ItemSlot> slots = [];
            for (int n = 0; n < 9; n++)
            {
                if((targetEntity as EntityPlayer)?.Player.InventoryManager.Inventories.Values.OfType<BackpackInventory>().First().GetSlotByBackpackSlotId("vanilla@self0@" + n, out var slot) == true) slots.Add( slot);
            }
            var stacks = slots.Select(slot => slot.Itemstack).ToArray();
            // ItemStack?[]? stacks = this.GetCollectibleBehavior<CollectibleBehaviorHeldBackpackBase>(false)?.GetContents(itemStack, Api.World);
            if (attachedShape != null && targetAtlas != null) //&& stacks != null)
                foreach (ItemStack? stack in stacks)
                {
                    string childPath = stack?.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, stack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(Api, childPath) is { } childShape)
                    {
                        childShape.Elements[0].StepParentName = "backpack" + i;
                        attachedShape.StepParentShape(childShape, texturePrefixCode, childPath, _attachedShapePath, Api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((Api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        shape?.StepParentShape(childShape, texturePrefixCode, childPath, _attachedShapePath, Api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((Api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                    }
                    i += 1;
                }
            if (collectedTextures != null) TexSource = new ShapeTextureSource((Api as ICoreClientAPI)!, shape!, "mylog", collectedTextures, (p) => p);
            Combshape = shape;
            return attachedShape;
        }
        public void RefreshShape(ItemStack itemStack, string? texturePrefixCode = null)
        {
            int i = 1;
            Shape? shape = _baseShape!.Clone();
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (Api as ICoreClientAPI)?.ItemTextureAtlas;
            ItemStack?[]? stacks = this.GetCollectibleBehavior<CollectibleBehaviorHeldBackpackBase>(false)?.GetContents(itemStack, Api.World);
            if (targetAtlas != null && stacks != null)
                foreach (ItemStack? stack in stacks)
                {
                    string childPath = stack?.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, stack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(Api, childPath) is { } childShape)
                    {
                        childShape.Elements[0].StepParentName = "backpack" + i;
                        shape!.StepParentShape(childShape, texturePrefixCode, childPath, _attachedShapePath, Api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((Api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                    }
                    i += 1;
                }
            if (collectedTextures != null) TexSource = new ShapeTextureSource((Api as ICoreClientAPI)!, shape, "mylog", collectedTextures, (p) => p);
            Combshape = shape;
        }
    }
}
