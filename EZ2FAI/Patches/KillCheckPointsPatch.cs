using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch]
    public static class KillCheckPointsPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]
        public static bool doEffect()
        {
            return Main.Settings.IsCheckPoint;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
        public static void Awake_Rewind()
        {
            var checkPoints = UnityEngine.Object.FindObjectsOfType<ffxCheckpoint>();
            if (checkPoints == null) return;
            if (!Main.Settings.IsCheckPoint)
            {
                foreach (var checkPoint in checkPoints)
                    checkPoint.floor.floorIcon = FloorIcon.None;
            }
            else
            {
                foreach (var checkPoint in checkPoints)
                    checkPoint.floor.floorIcon = FloorIcon.Checkpoint;
            }
        }
    }
}
