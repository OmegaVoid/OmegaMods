using HarmonyLib;
using System.Diagnostics;
using Vintagestory.API.Client;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase.HarmonyPatches
{
    
    [HarmonyPatch(typeof(BlockEntityGroundStorage), "OnTesselation")]
    public static class BERenderingPatch
    {
        //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original, ILGenerator iLGenerator)
        //{
        //    CodeMatcher codeMatcher = new CodeMatcher(instructions, iLGenerator).MatchStartForward(new CodeMatch(OpCodes.Ldstr, "Stacking model shape for collectible ")).Advance(-7);
        //    var helpervar = AccessTools.Method(typeof(BERenderingPatch), "helper");
        //    codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1));
        //    codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0));
        //    var helpervar2 = AccessTools.Field(typeof(BlockEntityGroundStorage), "nowTesselatingShape");
        //    codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, helpervar2));
        //    codeMatcher.InsertAndAdvance(new CodeInstruction(OpCodes.Call, helpervar));
        //    return codeMatcher.InstructionEnumeration();
        //}
        //static void helper(ItemSlot slot, Shape nowTesselatingShape)
        //{
        //    Debug.WriteLine(slot.Itemstack.Item.Class);
        //    if (slot.Itemstack?.Item is ItemBackpackBase backpack)
        //    {
        //        Debug.WriteLine("----------------------------------------------");
        //        nowTesselatingShape = backpack._shape;
        //    }
        //}
        public static bool Prefix(BlockEntityGroundStorage __instance, ITerrainMeshPool mesher, ITesselatorAPI tesselator)
        {
            //Debug.WriteLine("111111111111111111111111111111111111111111111111");
            if (__instance.Inventory.FirstNonEmptySlot?.Itemstack?.Item is ItemBackpackBase backpack)
            {
                //Debug.WriteLine("----------------------------------------------");
                var tryed = (__instance.Api as ICoreClientAPI)!.ObjectCache.TryGetValue("groundstorage-mesh-" + __instance.Pos.ToString(), out var _mesh);
                var mesh = _mesh as MeshData;
                if (!tryed)
                {
                    Debug.WriteLine("----------------------------------------------");
                    Debug.WriteLine(backpack._shape.Elements[0].Name);
                    tesselator.TesselateShape(backpack, backpack._shape, out mesh);
                    Debug.WriteLine("55555555555555555555555555555555555");
                    //mesh.TextureIds[0] = (__instance.Api as ICoreClientAPI).BlockTextureAtlas.AtlasTextures[0].TextureId;
                    (__instance.Api as ICoreClientAPI)!.ObjectCache.Add("groundstorage-mesh-" + __instance.Pos.ToString(), mesh);
                }
                mesher.AddMeshData(mesh);
            }
            return false;
        }
    }
}
