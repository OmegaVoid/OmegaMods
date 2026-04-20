using ConfigLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VSImGui;

namespace BackpackOverhaul
{
    
    /// <inheritdoc />
    public partial class BackpackOverhaulModSystem : ModSystem
    {
        private ImGuiModSystem _modSystem = null!;
        /// <summary>
        /// 
        /// </summary>
        public readonly BackpackOverhaulConfig Settings = new();

        /// <inheritdoc />
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            AutoSetup(api);
        }

        /// <inheritdoc />
        public override void Start(ICoreAPI api)
        {
            if (api.ModLoader.IsModEnabled("configlib"))
            {
                SubscribeToConfigChange(api);
            }
        }
        
        private void SubscribeToConfigChange(ICoreAPI api)
        {
            var system = api.ModLoader.GetModSystem<ConfigLibModSystem>();

            system.SettingChanged += (domain, config, setting) =>
            {
                
                if (domain != "thaumaturgy") return;
                setting.AssignSettingValue(Settings);
            };
            system.ConfigsLoaded += () =>
            {
                system.GetConfig("thaumaturgy")?.AssignSettingsValues(Settings);
            };
        }

        /// <inheritdoc />
        public override void StartClientSide(ICoreClientAPI api)
        {
            _modSystem = api.ModLoader.GetModSystem<ImGuiModSystem>();
        }
        
        /// <inheritdoc />
        public override void AssetsLoaded(ICoreAPI api)
        {
            base.AssetsLoaded(api);
            AutoAssetsLoaded(api);
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            AutoDispose();
            base.Dispose();
        }
    }
}