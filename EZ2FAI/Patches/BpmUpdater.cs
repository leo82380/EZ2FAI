using HarmonyLib;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace EZ2FAI.Patches
{
    [HarmonyPatch(typeof(scnGame), "Play")]
    public static class BpmUpdater_1
    {
        public static void Postfix(scnGame __instance)
        {
            if (!(scrController.instance?.gameworld ?? false)) return;
            if (scnGame.instance == null) return;
            BpmUpdater.Init(scrController.instance);
        }
    }
    [HarmonyPatch(typeof(scrPressToStart), "ShowText")]
    public static class BpmUpdater_2
    {
        public static void Postfix(scrPressToStart __instance)
        {
            if (!scrController.instance.gameworld) return;
            if (scnGame.instance != null) return;
            BpmUpdater.Init(scrController.instance);
        }
    }
    [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
    public static class BpmUpdater_3
    {
        public static void Postfix(scrPlanet __instance, scrFloor floor)
        {
            if (!scrController.instance.gameworld) return;
            Variables.CurrentCheckPoint = GetCheckPointIndex(floor);
            if (floor.nextfloor == null) return;
            double curBPM = GetRealBpm(floor, BpmUpdater.bpm) * BpmUpdater.playbackSpeed * BpmUpdater.pitch;
            bool isDongta = false;
            Variables.TileBpm = BpmUpdater.bpm * scrController.instance.speed;
            if (isDongta || BpmUpdater.beforedt) curBPM = BpmUpdater.beforebpm;
            Variables.CurBpm = curBPM;
            Variables.RecKPS = curBPM / 60;
            BpmUpdater.beforedt = isDongta;
            BpmUpdater.beforebpm = curBPM;

            curBPM = GetRealBpm(floor, BpmUpdater.bpmwithoutpitch) * BpmUpdater.playbackSpeed;
            Variables.TileBpmWithoutPitch = BpmUpdater.bpmwithoutpitch * scrController.instance.speed;
            Variables.CurBpmWithoutPitch = curBPM;
            Variables.RecKPSWithoutPitch = curBPM / 60;

            Main.Panel.curBPMText.text = Math.Round(curBPM, 2).ToString();
            Main.Panel.realBPMText.text = Math.Round(Variables.TileBpm, 2).ToString();
        }
        public static int GetCheckPointIndex(scrFloor floor)
        {
            int i = 0;
            foreach (var chkPt in BpmUpdater.AllCheckPoints)
            {
                if (floor.seqID + 1 <= chkPt.seqID)
                    return i;
                i++;
            }
            return i;
        }
        public static double GetRealBpm(scrFloor floor, float bpm)
        {
            if (floor == null)
                return bpm;
            if (floor.nextfloor == null)
                return scrController.instance.speed * bpm;
            return 60.0 / (floor.nextfloor.entryTime - floor.entryTime);
        }
    }
    public static class BpmUpdater
    {
        public static List<scrFloor> AllCheckPoints = new List<scrFloor>();
        public static FieldInfo curSpd = typeof(GCS).GetField("currentSpeedTrial", AccessTools.all);
        public static float bpm = 0, pitch = 0, playbackSpeed = 1, bpmwithoutpitch;
        public static bool beforedt = false;
        public static double beforebpm = 0;
        public static void Init(scrController __instance)
        {
            AllCheckPoints = scrLevelMaker.instance.listFloors.FindAll(f => f.GetComponent<ffxCheckpoint>() != null);
            Main.Panel.curBPMText.text = "0";
            Main.Panel.realBPMText.text = "0";
            float kps = 0;
            try
            {
                if (scnGame.instance != null)
                {
                    pitch = (float)scnGame.instance.levelData.pitch / 100;
                    if (ADOBase.isCLSLevel) pitch *= (float)curSpd.GetValue(null);
                    bpm = scnGame.instance.levelData.bpm * playbackSpeed * pitch;
                    bpmwithoutpitch = scnGame.instance.levelData.bpm * playbackSpeed;
                }
                else
                {
                    pitch = scrConductor.instance.song.pitch;
                    bpm = scrConductor.instance.bpm * pitch;
                    bpmwithoutpitch = scrConductor.instance.bpm;
                }
                playbackSpeed = scnEditor.instance?.playbackSpeed ?? 1;
            }
            catch
            {
                pitch = scrConductor.instance.song.pitch;
                playbackSpeed = 1;
                bpm = scrConductor.instance.bpm * pitch;
                bpmwithoutpitch = scrConductor.instance.bpm;
            }
            float cur = bpm;
            if (__instance.currentSeqID != 0)
            {
                double speed = scrController.instance.speed;
                cur = (float)(bpm * speed);
            }
            Variables.TileBpm = cur;
            Variables.CurBpm = cur;
            Variables.RecKPS = kps;
        }
    }
}

