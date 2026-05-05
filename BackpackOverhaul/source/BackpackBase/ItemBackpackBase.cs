using BackpackOverhaul.BackpackSystem;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Common;
using Vintagestory.GameContent;
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BackpackOverhaul.BackpackBase
{
    public class ItemBackpackBase : Item, IWearableShapeSupplier
    {
        public Shape? shape { get; private set; }
        public new ICoreAPI api => base.api;
        public BlockEntityGroundStorage? beGroundStorage;
        private Shape? attachedShape;
        private string? attachedShapePath;
        private string? baseShapePath;
        private Shape? baseShape;
        public bool dirty;
        public ItemStack? dirtyStack;
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
            if (!CompareShapes(shape, _shape))
            {
                shape = _shape;
                dirty = true;
            }
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
            Vintagestory.API.Util.EqualityUtil.NumberEquals(shape, _shape);
            if (!CompareShapes(shape, _shape))
            {
                shape = _shape;
                dirty = true;
            }
        }
        private bool CompareShapes(Shape? shape1, Shape? shape2)
        {
            string[]? fullshape1 = GetAllShapeElements(shape1!);
            string[]? fullshape2 = GetAllShapeElements(shape2!);
            bool comp = true;
            if (fullshape1 == null & fullshape2 == null) return true;
            if (fullshape1 != null & fullshape2 == null) return false;
            if (fullshape1 == null & fullshape2 != null) return false;
            fullshape1?.ToList().ForEach(element =>
            {
                if (!fullshape2.Contains(element)) comp = false;
            });
            fullshape2?.ToList().ForEach(element =>
            {
                if (!fullshape1.Contains(element)) comp = false;
            });
            return comp;
        }
        private string[]? GetAllShapeElements(Shape shape)
        {
            List<string> elements = new List<string>();
            shape?.Elements.ToList().ForEach(element =>
            {
                elements.Add(element.ToString());
                if (GetElementChildren(element) is string[] children) elements.AddRange(children);
            });
            return elements.ToArray();
        }
        private string[]? GetElementChildren(ShapeElement element)
        {
            List<string> elements = new List<string>();
            element.Children?.ToList().ForEach(child =>
            {
                if(child != null) 
                { 
                    elements.Add(child.ToString()); 
                    if (GetElementChildren(child) is string[] children) elements.AddRange(children);
                }
            });
            return elements.ToArray();
        }
    }
}
