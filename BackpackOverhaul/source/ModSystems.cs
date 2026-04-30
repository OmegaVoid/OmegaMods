using InsanityLib.Generators.Attributes;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VSImGui;
[assembly: AutoRegistryName(RemovePrefix = ["CollectibleBehavior", "Block", "EntityBehavior", "Entity", "BlockEntity", "BlockEntiityBehavior", "BlockBehavior", "Item"])]
namespace BackpackOverhaul
{
    public partial class BackpackOverhaulModSystem : ModSystem
    {
        private ImGuiModSystem _modSystem = null!;
        public readonly BackpackOverhaulConfig Settings = new();
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            AutoSetup(api);
        }
        public override void StartClientSide(ICoreClientAPI api)
        {
            _modSystem = api.ModLoader.GetModSystem<ImGuiModSystem>();
        }    
        public override void AssetsLoaded(ICoreAPI api)
        {
            base.AssetsLoaded(api);
            AutoAssetsLoaded(api);
        }
        public override void Dispose()
        {
            AutoDispose();
            base.Dispose();
        }
    }
}