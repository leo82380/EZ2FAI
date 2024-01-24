using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch(typeof(scrController), "Hit")]
    public static class HitPatch
    {
        public static void Postfix(scrController __instance)
        {
            if (scrController.instance.gameworld)
            {
                Main.Panel.SetJudgeAccuracy(__instance);
                if (!Main.Settings.SongProgress)
                    Main.Panel.SetProgress(__instance.mistakesManager.percentComplete);
            }
        }
    }
}
