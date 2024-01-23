using HarmonyLib;

public class Patches
{
    public static Patches instance;
    public static HitMargin GetHitMargin(float angle)
    {
        double bpmTimesSpeed = scrConductor.instance.bpm * scrController.instance.speed;
        double conductorPitch = scrConductor.instance.song.pitch;
        double counted =
            scrMisc.GetAdjustedAngleBoundaryInDeg(
                HitMarginGeneral.Counted, bpmTimesSpeed, conductorPitch);
        double perfect =
            scrMisc.GetAdjustedAngleBoundaryInDeg(
                HitMarginGeneral.Perfect, bpmTimesSpeed, conductorPitch);
        double pure =
            scrMisc.GetAdjustedAngleBoundaryInDeg(
                HitMarginGeneral.Pure, bpmTimesSpeed, conductorPitch);
        if (angle < -counted)
        {
            return HitMargin.TooEarly;
        }
        else if (angle < -perfect)
        {
            return HitMargin.VeryEarly;
        }
        else if (angle < -pure)
        {
            return HitMargin.EarlyPerfect;
        }
        else if (angle <= pure)
        {
            return HitMargin.Perfect;
        }
        else if (angle <= perfect)
        {
            return HitMargin.LatePerfect;
        }
        else if (angle <= counted)
        {
            return HitMargin.VeryLate;
        }
        else
        {
            return HitMargin.TooLate;
        }
    }
    public static HitMargin GetHitMarginForDifficulty(float angle, Difficulty difficulty)
    {
        Difficulty temp = GCS.difficulty;
        GCS.difficulty = difficulty;
        HitMargin margin = GetHitMargin(angle);
        GCS.difficulty = temp;
        return margin;
    }
    public static string nmargin;
    public static string smargin;
    public static string lmargin;
    public static float angle;
    public static HitMargin Lenient;
    public static HitMargin Normal;
    public static HitMargin Strict;


    [HarmonyPatch(typeof(scrMisc), "GetHitMargin")]
    public class GetHitMarginPatch
    {
        public static bool Prefix(float hitangle, float refangle, bool isCW, float bpmTimesSpeed, float conductorPitch, ref HitMargin __result)
        {
            float num = (hitangle - refangle) * (float)(isCW ? 1 : -1);
            HitMargin result = HitMargin.TooEarly;
            float num2 = num;
            num2 = 57.29578f * num2;
            double adjustedAngleBoundaryInDeg = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Counted, (double)bpmTimesSpeed, (double)conductorPitch);
            double adjustedAngleBoundaryInDeg2 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Perfect, (double)bpmTimesSpeed, (double)conductorPitch);
            double adjustedAngleBoundaryInDeg3 = scrMisc.GetAdjustedAngleBoundaryInDeg(HitMarginGeneral.Pure, (double)bpmTimesSpeed, (double)conductorPitch);
            if ((double)num2 > -adjustedAngleBoundaryInDeg)
            {
                result = HitMargin.VeryEarly;
            }
            if ((double)num2 > -adjustedAngleBoundaryInDeg2)
            {
                result = HitMargin.EarlyPerfect;
            }
            if ((double)num2 > -adjustedAngleBoundaryInDeg3)
            {
                result = HitMargin.Perfect;
            }
            if ((double)num2 > adjustedAngleBoundaryInDeg3)
            {
                result = HitMargin.LatePerfect;
            }
            if ((double)num2 > adjustedAngleBoundaryInDeg2)
            {
                result = HitMargin.VeryLate;
            }
            if ((double)num2 > adjustedAngleBoundaryInDeg)
            {
                result = HitMargin.TooLate;
            }
            Lenient = GetHitMarginForDifficulty(num2, Difficulty.Lenient);
            Normal = GetHitMarginForDifficulty(num2, Difficulty.Normal);
            Strict = GetHitMarginForDifficulty(num2, Difficulty.Strict);
            lmargin = RDString.Get("HitMargin." + Lenient.ToString());
            nmargin = RDString.Get("HitMargin." + Normal.ToString());
            smargin = RDString.Get("HitMargin." + Strict.ToString());
            JTM.lcounts[Lenient]++;
            JTM.ncounts[Normal]++;
            JTM.scounts[Strict]++;
            angle = num2;
            __result = result;
            return false;
        }
    }
    [HarmonyPatch(typeof(ffxCheckpoint), "doEffect")]
    public static class EffectPatch
    {
        private static bool Prefix()
        {
            return false;
        }
    }
}