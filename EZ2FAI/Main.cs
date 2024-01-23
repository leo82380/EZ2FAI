using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager.ModEntry;
using HarmonyLib;
using System.Reflection;
using TMPro;
using ADOFAI;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;

public class Main
{
    public static Harmony harmony;
    public static ModLogger Logger;
    public static bool Modon = false;
    public static bool isRunning = false;

    public static string username = "";
    public static string usernametemp = "";

    public static string level_auther = "";
    public static string level_song = "";
    public static string level_artist = "";
    public static string level_songtist = "";

    public static float play_XA = 0;

    public static int play_TE = 0;
    public static int play_VE = 0;
    public static int play_EP = 0;
    public static int play_P = 0;
    public static int play_LP = 0;
    public static int play_VL = 0;
    public static int play_TL = 0;

    public static string nmargin;
    public static string smargin;
    public static string lmargin;
    public static float angle;
    public static HitMargin Lenient;
    public static HitMargin Normal;
    public static HitMargin Strict;

    public static GameObject mp;
    public static TextMeshProUGUI userNameTxt;
    public static TextMeshProUGUI autherTxt;
    public static TextMeshProUGUI songtistTxt;
    public static TextMeshProUGUI judgeRateTxt;
    public static TextMeshProUGUI[] judgeTexts = new TextMeshProUGUI[7];
    public static Image progressImage;
    public static AssetBundleLoadResult result;
    public static AssetBundle asset;

    public static float X = 0.16f;
    public static float Y = 0.1f;
    public static float S = 0.7f;


    public static bool Start(UnityModManager.ModEntry modEntry)
    {
        Logger = modEntry.Logger;
        username = "Guest-" + RandomStringGenerator.GenerateRandomString(8);
        usernametemp = username;
        

        modEntry.OnToggle = (entry,value) =>
        {
            Modon = value;
            if(value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                InitializeMP();
            }
            else
            {
                UnLoad(Path.GetDirectoryName(modEntry.Path));
                if(isRunning)
                {
                    isRunning = false;
                }
            }

            return true;
        };

        void InitializeMP()
        {
            try
            {
                asset = LoadAssetBundle(Path.GetDirectoryName(modEntry.Path));
                mp = UnityEngine.Object.Instantiate(asset.LoadAsset<GameObject>("MPCanvas 12"));
                UnityEngine.Object.DontDestroyOnLoad(mp);

                MPSet mpSet = new GameObject().AddComponent<MPSet>();
                UnityEngine.Object.DontDestroyOnLoad(mpSet);

                userNameTxt = mp.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                autherTxt = mp.transform.GetChild(0).GetChild(4).GetComponent<TextMeshProUGUI>();
                songtistTxt = mp.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>();
                judgeRateTxt = mp.transform.GetChild(0).GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>();

                for(int i = 0;i <= 6;i++)
                {
                    judgeTexts[i] = mp.transform.GetChild(0).GetChild(6).GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>();
                }
                progressImage = mp.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();

                autherTxt.text = Main.level_auther;
                songtistTxt.text = Main.level_songtist;
                userNameTxt.text = username;
            }
            catch(Exception ex)
            {
                Logger.Error($"Exception during InitializeMP: {ex}");
            }
        }

        modEntry.OnGUI = (entry) =>
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:",GUILayout.Width(0));
            if(GUILayout.Button("Apply",GUILayout.Width(120)))
            {
                username = usernametemp;
                userNameTxt.text = username; 
            }

            usernametemp = GUILayout.TextField(usernametemp);
            GUILayout.EndHorizontal();

            GUILayout.Label("P1BoxSet");
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:", GUILayout.Width(20));
            X = float.Parse(GUILayout.TextField(X.ToString(), 6, GUILayout.Width(80)));
            X = GUILayout.HorizontalSlider(X, -0.4f, 1.4f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Y:", GUILayout.Width(20));
            Y = float.Parse(GUILayout.TextField(Y.ToString(), 6, GUILayout.Width(80)));
            Y = GUILayout.HorizontalSlider(Y, -0.2f, 1.2f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("S:", GUILayout.Width(20));
            S = float.Parse(GUILayout.TextField(S.ToString(), 6, GUILayout.Width(80)));
            S = GUILayout.HorizontalSlider(S, 0f, 2f);
            GUILayout.EndHorizontal();
        };


        return true;
    }
    //public static GameObject AssetBundleLoad(string modPath)
    //{
    //    try
    //    {
    //        GameObject a = LoadAssetBundle(modPath).LoadAsset<GameObject>("MPCanvas");
    //        return a;
    //    }
    //    catch(Exception e)
    //    {
    //        Logger.Error(e.Message);
    //        GameObject a = LoadAssetBundle(modPath).LoadAsset<GameObject>("MPCanvas");
    //        return a;
    //    }
    //}


    public static AssetBundle LoadAssetBundle(string modPath)
    {
        string path = Path.Combine(modPath, "mp");
        try
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            return assetBundle;
        }
        catch (Exception e)
        {
            Logger.Error($"Error loading asset bundle {modPath}");
            Logger.Error(e.Message);
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            return assetBundle;
        }
    }

    public static void UnLoad(string modPath)
    {
        try
        {
            UnityEngine.Object.Destroy(mp);
            asset.Unload(true);
        }
        catch (Exception e)
        {
            Logger.Log("=================");
            Logger.Error(e.Message);
            UnityEngine.Object.Destroy(mp); 
        }
    }

}

public class RandomStringGenerator
{
    public static readonly System.Random random = new System.Random();
    public const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length)
    {
        char[] randomString = new char[length];

        for(int i = 0;i < length;i++)
        {
            randomString[i] = chars[random.Next(chars.Length)];
        }

        return new string(randomString);
    }
}

[HarmonyPatch(typeof(LevelData),"LoadLevel")]
public static class Levelload
{
    public static void Postfix(LevelData __instance)
    {
        Main.level_auther = DeleteRichTag(__instance.author);
        Main.level_artist = DeleteRichTag(__instance.artist);
        Main.level_song = DeleteRichTag(__instance.song);
        Main.level_songtist = Main.level_artist + " - " + Main.level_song;

        Main.autherTxt.text = Main.level_auther;
        Main.songtistTxt.text = Main.level_songtist;

    }
    public static readonly Regex DelTag = new Regex(@"<(color|material|quad|size)=(.|\n)*?>|<\/(color|material|quad|size)>|<(b|i)>|<\/(b|i)>",RegexOptions.Compiled | RegexOptions.Multiline);
    public static string DeleteRichTag(this string s)
            => DelTag.Replace(s,string.Empty);
}

[HarmonyPatch(typeof(scrController),"Hit")]
public class Hit
{
    public static float play_XA_temp = 0f;
    public static void Postfix()
    {
        if(scrController.instance.gameworld)
        {
            Main.play_XA = (float)Math.Round(scrController.instance.mistakesManager.percentXAcc * 100d,2);
            Main.judgeRateTxt.text = Main.play_XA.ToString() + "%";
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

public class MPSet : MonoBehaviour
{
    private float deltaTime = 0f;
    void Update()
    {
        try
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            if (scrConductor.instance == null || Main.mp == null)
            {
                return;
            }
            if (scrConductor.instance.isGameWorld && !scrConductor.isEditingLevel && !scrConductor.isOfficialLevel)
            {
                Main.mp.SetActive(true);
                var checkPoints = FindObjectsOfType<ffxCheckpoint>();
                foreach (var checkPoint in checkPoints)
                {
                    checkPoint.floor.floorIcon = FloorIcon.None;
                }
                Main.mp.transform.GetChild(0).position = new Vector2(Main.X * Screen.width, Main.Y * Screen.height);
                Main.mp.transform.GetChild(0).localScale = new Vector3(Main.S, Main.S, 1f);
                //float ms = deltaTime * 1000f;
                //float fps = 1.0f / deltaTime;
                //string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
                //Main.mp.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;

            }
            else
            {
                Main.mp.SetActive(false);
            }
        }
        catch (Exception e) 
        {
            Debug.Log("=======================================");
            Debug.Log(e.Message);
            return;
        }

    }




}



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
        Main.level_auther = "";
        Main.level_artist = "";
        Main.level_song = "";
        Main.level_songtist = "";

        Main.autherTxt.text = Main.level_auther;
        Main.songtistTxt.text = Main.level_songtist;
    }
    public static void ResetJudgetxt()
    {
        JTM.ResetJTM();
        Main.play_XA = 0;
        Main.judgeRateTxt.text = "NaN%";
        Main.progressImage.fillAmount = 0f;

        foreach (var s in Main.judgeTexts)
        {
            s.text = "0";
        }
    }
}
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
