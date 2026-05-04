using HarmonyLib;
using System.Diagnostics;
using System.Security.AccessControl;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

namespace BackpackOverhaul.BackpackBase.HarmonyPatches
{

    [HarmonyPatch(typeof(CollectibleBehaviorHeldBag), "GetOrCreateSlots")]
    public static class CollectibleBehaviorHeldBagPatch
    {
        public static void Postfix(CollectibleBehaviorHeldBag __instance, List<ItemSlotBagContent> __result, ItemStack bagstack, InventoryBase parentinv, int bagIndex, IWorldAccessor world)
        {
            if (__instance.collObj is ItemBackpackBase backpack)
            {
                __result[0].Inventory.SlotModified += (int i) =>
                {
                    backpack.dirty = true;
                    parentinv.MarkSlotDirty(bagIndex);
                    if(parentinv.ClassName == "blockcontainedbaginv")
                    {
                        backpack.RefreshShape(bagstack);
                        try { backpack.beGroundStorage?.MarkDirty(true); }
                        catch (Exception e) { Debug.WriteLine(e.Message); }
                        
                    }
                };
            }
        }
    }
}
