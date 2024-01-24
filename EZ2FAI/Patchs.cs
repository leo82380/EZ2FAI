using HarmonyLib;

public class Patches
{
    [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]
    public static class EffectPatch
    {
        private static bool Prefix()
        {
            return false;
        }
    }
}