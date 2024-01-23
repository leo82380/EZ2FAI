using System.Collections.Generic;
using HarmonyLib;

public static class JTM
{
    public static Harmony harmony;
    public static Dictionary<HitMargin,int> ncounts = new Dictionary<HitMargin,int>() {
            { HitMargin.Perfect, 0 },
            { HitMargin.EarlyPerfect, 0 },
            { HitMargin.LatePerfect, 0 },
            { HitMargin.VeryEarly, 0 },
            { HitMargin.VeryLate, 0 },
            { HitMargin.TooEarly, 0 },
            { HitMargin.TooLate, 0 }
        };
    public static Dictionary<HitMargin,int> scounts = new Dictionary<HitMargin,int>() {
            { HitMargin.Perfect, 0 },
            { HitMargin.EarlyPerfect, 0 },
            { HitMargin.LatePerfect, 0 },
            { HitMargin.VeryEarly, 0 },
            { HitMargin.VeryLate, 0 },
            { HitMargin.TooEarly, 0 },
            { HitMargin.TooLate, 0 }
        };
    public static Dictionary<HitMargin,int> lcounts = new Dictionary<HitMargin,int>() {
            { HitMargin.Perfect, 0 },
            { HitMargin.EarlyPerfect, 0 },
            { HitMargin.LatePerfect, 0 },
            { HitMargin.VeryEarly, 0 },
            { HitMargin.VeryLate, 0 },
            { HitMargin.TooEarly, 0 },
            { HitMargin.TooLate, 0 }
        };
    public static bool IsEnabled { get; private set; }
    public static HitMargin[] hitmargins = new HitMargin[7] { HitMargin.Perfect,HitMargin.LatePerfect,HitMargin.EarlyPerfect,HitMargin.VeryEarly,HitMargin.VeryLate,HitMargin.TooEarly,HitMargin.TooLate };
    public static void ResetJTM()
    {
        foreach(HitMargin h in hitmargins)
        {
            ncounts[h] = 0;
            lcounts[h] = 0;
            scounts[h] = 0;
        }
    }
    public static string CurMargin(HitMargin hit)
    {
        string result = string.Empty;
        if(GCS.difficulty == Difficulty.Lenient)
        {
            result = lcounts[hit].ToString();
        }
        if(GCS.difficulty == Difficulty.Normal)
        {
            result = ncounts[hit].ToString();
        }
        if(GCS.difficulty == Difficulty.Strict)
        {
            result = scounts[hit].ToString();
        }
        return result;
    }
}
