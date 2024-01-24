using System.Collections.Generic;
using HarmonyLib;

public static class JTM
{
    public static Harmony harmony;
    
    public static bool IsEnabled { get; private set; }
    public static string CurMargin(HitMargin hit)
    {
        return scrController.instance.mistakesManager.GetHits(hit).ToString();
    }
}
