using ConfigLib;
using ImGuiNET;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;
using VSImGui;
using VSImGui.API;
using YamlDotNet.Serialization;

namespace Thaumaturgy
{
    public sealed class ThaumaturgyModSystem : ModSystem
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
            // _modSystem.Draw += Draw;
        }
        
        // private CallbackGUIStatus Draw(float deltaSeconds)
        // {
        //     ImGui.Begin("ImGui example");
        //
        //     var roll = 0f;
        //     ImGui.SliderFloat("Roll", ref roll, -90, 90);
        //
        //     ImGui.End();
        //     return CallbackGUIStatus.GrabMouse;
        // }
    }
}