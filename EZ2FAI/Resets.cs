using HarmonyLib;

public class Resets
{
    [HarmonyPatch(typeof(scrController),"FailAction")] //사망 시
    public class FailActionPatch
    {
        public static void Postfix()
        {
        }
    }

    [HarmonyPatch(typeof(scrController),"QuitToMainMenu")] //메뉴로 나갈 시
    public class QuitToMainMenuPatch
    {
        public static void Postfix()
        {
            ResetJudgetxt();
            ResetLvltxt();
        }
    }

    [HarmonyPatch(typeof(scrUIController),"WipeFromBlack")] //맵 재시작 시
    public class WipeFromBlackPatch
    {
        public static void Postfix()
        {
            if (scnGame.instance.checkpointsUsed == 0)
            {
                ResetJudgetxt();
            }
        }
    }

    [HarmonyPatch(typeof(scrController),"ResetCustomLevel")] //커스텀래밸 리셋 시
    public static class ResetCustomLevelPatch
    {
        public static void Postfix()
        {
            if (scnGame.instance.checkpointsUsed == 0)
            {
                ResetJudgetxt();
            }
        }
    }

    [HarmonyPatch(typeof(scnEditor),"ResetScene")] //에디터씬 리셋 시
    public static class ResetScenePatch
    {
        public static void Postfix()
        {
            ResetJudgetxt();
        }
    }

    public static void ResetLvltxt()
    {
        Main.level_author = "";
        Main.level_artist = "";
        Main.level_song = "";
        Main.level_songtist = "";

        Main.autherTxt.text = Main.level_author;
        Main.songtistTxt.text = Main.level_songtist;
    }

    public static void ResetJudgetxt()
    {
        Main.play_XA = 0;
        Main.judgeRateTxt.text = "NaN%";
        Main.progressImage.fillAmount = 0f;

        foreach (var s in Main.judgeTexts)
        {
            s.text = "0";
        }
    }
}