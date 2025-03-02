using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch]
    public static class ResetPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "QuitToMainMenu")]
        public static void QuitToMainMenu()
        {
            Main.Panel.ResetJudgeAccuracy();
            Main.Panel.ResetProgress();
            Main.Panel.ResetMapName();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrUIController), "WipeFromBlack")]
        public static void WipeFromBlack()
        {
            Main.Panel.ResetJudgeAccuracy();
            Main.Panel.ResetProgress();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scrController), "ResetCustomLevel")]
        public static void ResetCustomLevel()
        {
            Main.Panel.ResetJudgeAccuracy();
            Main.Panel.ResetProgress();
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(scnEditor), "ResetScene")]
        public static void ResetScene()
        {
            Main.Panel.ResetJudgeAccuracy();
            Main.Panel.ResetProgress();
        }
    }
}
