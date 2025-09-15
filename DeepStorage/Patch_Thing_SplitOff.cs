using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace LWM.DeepStorage
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.SplitOff))]
    public static class Patch_Thing_SplitOff
    {
        static bool Prefix(Thing __instance, ref Thing __result, int count)
        {
            if (count <= 0)
            {
                __result = __instance;
                return false;
            }


            try
            {
                if (__instance.Spawned && __instance.def.category == ThingCategory.Item)
                {
                    __instance.Map?.GetComponent<MapComponentDS>()?.DirtyCache(__instance.Position);
                }
            }
            catch
            {
            }
            return true;
        }

        static Exception Finalizer(Thing __instance, int count, ref Thing __result, Exception __exception)
        {
            if (__exception == null) return null;

            if (__exception is ArgumentException && count <= 0)
            {
                Log.Warning($"[DeepStorage] Caught SplitOff(count={count}) on {__instance}; returning original stack and continuing.");
                __result = __instance;
                return null;
            }

            return __exception;
        }
    }
}