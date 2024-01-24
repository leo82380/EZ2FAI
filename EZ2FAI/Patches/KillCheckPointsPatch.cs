using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch]
    public static class KillCheckPointsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]
        public static bool doEffect() => false;
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static void Awake_Rewind()
        {
            var checkPoints = UnityEngine.Object.FindObjectsOfType<ffxCheckpoint>();
            if (checkPoints == null) return;
            foreach (var checkPoint in checkPoints)
                checkPoint.floor.floorIcon = FloorIcon.None;
        }
    }
}
