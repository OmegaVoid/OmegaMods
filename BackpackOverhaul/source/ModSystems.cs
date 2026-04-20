using ConfigLib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using VSImGui;

namespace BackpackOverhaul
{
    public sealed class BackpackOverhaulModSystem : ModSystem
    {
        private ICoreAPI _api = null!;
        private ImGuiModSystem _modSystem = null!;
        public ModConfig Settings = new();

        public override void StartPre(ICoreAPI api)
        {
            _api = api;
            base.StartPre(api);
        }

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

        public override void StartClientSide(ICoreClientAPI api)
        {
            _modSystem = api.ModLoader.GetModSystem<ImGuiModSystem>();
        }
    }
}