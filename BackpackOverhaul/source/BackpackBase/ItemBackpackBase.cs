using BackpackOverhaul.BackpackSystem;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase : Item, IWearableShapeSupplier
    {
        public Shape? shape { get; private set; }
        public new ICoreAPI api => base.api;
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
            string[] p = ["W", "E", "top"];
            int i = 0;
            Shape? _attachedShape = attachedShape?.Clone();
            Shape? _shape = baseShape!.Clone();
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (api as ICoreClientAPI)?.ItemTextureAtlas;
            ItemStack[]? stacks = this.GetCollectibleBehavior<CollectibleBehaviorHeldBackpackBase>(false).GetContents(itemStack, api.World);
            if (_attachedShape != null & targetAtlas != null & stacks != null)
                foreach (ItemStack? stack in stacks!)
                {
                    string childPath = stack?.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, stack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(api, childPath) is Shape childShape)
                    {
                        childShape.Elements[0].StepParentName = p[i];
                        _attachedShape!.StepParentShape(childShape, texturePrefixCode, childPath, attachedShapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));
                        _shape!.StepParentShape(childShape, texturePrefixCode, childPath, attachedShapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));

                        i += 1;
                    }
                }
            if (collectedTextures != null) texSource = new ShapeTextureSource((api as ICoreClientAPI)!, _shape, "mylog", collectedTextures, (p) => p);
            shape = _shape;
            return _attachedShape;
        }
        public void RefreshShape(ItemStack itemStack, string texturePrefixCode = null)
        {
            string[] p = ["W", "E", "top"];
            int i = 0;
            Shape? _shape = baseShape!.Clone();
            IDictionary<string, CompositeTexture> collectedTextures = this.Textures;
            ITextureAtlasAPI? targetAtlas = (api as ICoreClientAPI)?.ItemTextureAtlas;
            ItemStack[]? stacks = this.GetCollectibleBehavior<CollectibleBehaviorHeldBackpackBase>(false).GetContents(itemStack, api.World);
            if (targetAtlas != null & stacks != null)
                foreach (ItemStack? stack in stacks!)
                {
                    string childPath = stack?.Item.Attributes["backpack"]["attachedShape"].AsObject<CompositeShape>(null, stack.Item.Code.Domain)?.Base.CopyWithPathPrefixAndAppendixOnce("shapes/", ".json") ?? "";
                    if (Vintagestory.API.Common.Shape.TryGet(api, childPath) is Shape childShape)
                    {
                        childShape.Elements[0].StepParentName = p[i];
                        _shape!.StepParentShape(childShape, texturePrefixCode, childPath, attachedShapePath, api.World.Logger, (texcode, tloc) => EntityBehaviorContainer.addTexture((api as ICoreClientAPI), texcode, tloc, collectedTextures, texturePrefixCode, targetAtlas));

                        i += 1;
                    }
                }
            if (collectedTextures != null) texSource = new ShapeTextureSource((api as ICoreClientAPI)!, _shape, "mylog", collectedTextures, (p) => p);
            shape = _shape;
        }
    }
}
