using System;
using HarmonyLib;

[HarmonyPatch(typeof(scrController),"Hit")]
public class Hit
{
    public static float play_XA_temp = 0f;
    public static void Postfix()
    {
        if(scrController.instance.gameworld)
        {
            Main.play_XA = (float)Math.Round(scrController.instance.mistakesManager.percentXAcc * 100d,2);
            Main.judgeRateTxt.text = Main.play_XA + "%";
            if(play_XA_temp != Main.play_XA)
            {
                play_XA_temp = Main.play_XA;
            }

            Main.judgeTexts[0].text = JTM.CurMargin(HitMargin.TooEarly);
            Main.judgeTexts[1].text = JTM.CurMargin(HitMargin.VeryEarly);
            Main.judgeTexts[2].text = JTM.CurMargin(HitMargin.EarlyPerfect);
            Main.judgeTexts[3].text = JTM.CurMargin(HitMargin.Perfect);
            Main.judgeTexts[4].text = JTM.CurMargin(HitMargin.LatePerfect);
            Main.judgeTexts[5].text = JTM.CurMargin(HitMargin.VeryLate);
            Main.judgeTexts[6].text = JTM.CurMargin(HitMargin.TooLate);

            Main.progressImage.fillAmount = scrController.instance.percentComplete;
        }

    }
}