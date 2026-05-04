using HarmonyLib;
using System.Diagnostics;
using System.Security.AccessControl;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase.HarmonyPatches
{

    [HarmonyPatch(typeof(BlockEntityGroundStorage), "OnTesselation")]
    public static class BERenderingPatche
    {
        public static bool Prefix(BlockEntityGroundStorage __instance, ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            if (__instance.Inventory.FirstNonEmptySlot?.Itemstack?.Item is ItemBackpackBase backpack)
             {
                backpack.beGroundStorage = __instance;
                if (backpack.dirty | !backpack.dirty) (__instance.Api as ICoreAPI)!.ObjectCache.Remove("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString());
                bool tryed = (__instance.Api as ICoreClientAPI)!.ObjectCache.TryGetValue("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString(), out var _mesh);
                var mesh = _mesh as MeshData;
                if (!tryed & backpack.texSource != null)
                {
                    tesselator.TesselateShape("mylog",backpack.shape, out mesh, backpack.texSource);
                    (__instance.Api as ICoreClientAPI)!.ObjectCache.Add("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString(), mesh);
                    backpack.dirty = false;
                }
                mesher.AddMeshData(mesh);
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(BlockEntityGroundStorage), "OnBlockRemoved")]
    public static class BERenderingPatch2
    {
        public static bool Prefix(BlockEntityGroundStorage __instance)
        {
            (__instance.Api as ICoreAPI)!.ObjectCache.Remove("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString());
            return false;
        }
    }
}
