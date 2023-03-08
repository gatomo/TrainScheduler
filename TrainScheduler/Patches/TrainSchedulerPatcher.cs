using HarmonyLib;
using UnityEngine;

namespace TrainScheduler.Patches
{
    public static class TrainSchedulerPatcher
    {
        private const string HarmonyId = "gatomo.TrainScheduler";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;

            Debug.Log("TrainScheduler Patching...");

            patched = true;
            var harmony = new Harmony(HarmonyId);
            harmony.PatchAll(typeof(TrainSchedulerPatcher).Assembly);
        }

        public static void UnpatchAll()
        {
            if (!patched) return;

            var harmony = new Harmony(HarmonyId);
            harmony.UnpatchAll(HarmonyId);
            patched = false;

            Debug.Log("TrainScheduler Reverted...");
        }
    }
}
