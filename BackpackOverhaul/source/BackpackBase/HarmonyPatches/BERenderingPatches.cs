using HarmonyLib;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase.HarmonyPatches
{

    [HarmonyPatch(typeof(BlockEntityGroundStorage), "OnTesselation")]
    public static class BeRenderingPatches
    {
        // ReSharper disable once InconsistentNaming
        public static bool Prefix(BlockEntityGroundStorage __instance, ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            if (__instance.Inventory.FirstNonEmptySlot?.Itemstack?.Item is ItemBackpackBase backpack)
            {
                backpack.RefreshShape(__instance.Inventory.FirstNonEmptySlot?.Itemstack!);
                //bool tryed = (__instance.Api as ICoreClientAPI)!.ObjectCache.TryGetValue("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString(), out var _mesh);
                //var mesh = _mesh as MeshData;
                MeshData? mesh = null;
                //if (!tryed & backpack.texSource != null)
                if (backpack.TexSource != null)
                {
                    tesselator.TesselateShape("mylog", backpack.Combshape, out mesh, backpack.TexSource);
                    //(__instance.Api as ICoreClientAPI)!.ObjectCache.Add("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString(), mesh);
                }
                if (mesh != null) mesher.AddMeshData(mesh);
            }
            return false;
        }
    }
}
        //[HarmonyPatch(typeof(BlockEntityGroundStorage), "OnBlockRemoved")]
        //public static class BERenderingPatch2
        //{
        //    public static bool Prefix(BlockEntityGroundStorage __instance)
        //    {
        //        //(__instance.Api as ICoreAPI)!.ObjectCache.Remove("groundstorage-mesh-backpackbase-" + __instance.Pos.ToString());
        //        return false;
        //    }
        //}
