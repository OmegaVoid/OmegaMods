using System.Reflection.Emit;
using HarmonyLib;
using PlayerInventoryLib;
using Vintagestory.API.Common;

namespace BackpackOverhaul.BackpackBase.HarmonyPatches;

    [HarmonyPatch(typeof(BackpackInventory), "OnItemSlotModified")]
    public static class BackpackInventoryPatch
    {
        public static void Postfix(BackpackInventory __instance, ItemSlot slot)
        {
            __instance.Player?.Entity?.MarkShapeModified();
        }
    }
