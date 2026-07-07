using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch(typeof(scrPlayer), "Hit")]
    public static class HitPatch
    {
        public static void Postfix(scrPlayer __instance)
        {
            if (scrController.instance.gameworld && __instance == scrController.instance.playerOne)
            {
                Main.Panel.SetJudgeAccuracy(__instance);
                if (!Main.Settings.SongProgress)
                    Main.Panel.SetProgress(__instance.marginTracker.percentComplete);
            }
        }
    }
}
