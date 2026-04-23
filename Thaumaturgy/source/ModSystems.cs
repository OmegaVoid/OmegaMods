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

namespace Thaumaturgy
{
    /// <inheritdoc />
    public partial class ThaumaturgyModSystem : ModSystem
    {
        private ImGuiModSystem _modSystem = null!;

        /// <summary>
        /// 
        /// </summary>
        public readonly ThaumaturgyConfig Settings = new();

        /// <inheritdoc />
        public override void StartPre(ICoreAPI api)
        {
            base.StartPre(api);
            AutoSetup(api);
        }

        /// <inheritdoc />
        public override void Start(ICoreAPI api)
        {
            if (api.ModLoader.IsModEnabled("configlib")) SubscribeToConfigChange(api);
        }

        private void SubscribeToConfigChange(ICoreAPI api)
        {
            var system = api.ModLoader.GetModSystem<ConfigLibModSystem>();

            system.SettingChanged += (domain, config, setting) =>
            {
                if (domain != "thaumaturgy") return;
                setting.AssignSettingValue(Settings);
            };
            system.ConfigsLoaded += () => system.GetConfig("thaumaturgy")?.AssignSettingsValues(Settings);
        }

        /// <inheritdoc />
        public override void StartClientSide(ICoreClientAPI api)
        {
            _modSystem = api.ModLoader.GetModSystem<ImGuiModSystem>();
            // _modSystem.Draw += Draw;
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