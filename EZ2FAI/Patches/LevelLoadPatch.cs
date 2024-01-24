using ADOFAI;
using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch(typeof(LevelData), "LoadLevel")]
    public static class LevelLoadPatch
    {
        public static void Postfix(LevelData __instance)
        {
            Main.Panel.SetMapName(__instance);
        }
    }
}
