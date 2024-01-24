using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace EZ2FAI.Patches
{
    [HarmonyPatch(typeof(scrConductor), "Update")]
    public static class SongProgressPatch
    {
        public static void Postfix(scrConductor __instance)
        {
            if (scnGame.instance && scrController.instance.gameworld && __instance.song.isPlaying && Main.Settings.SongProgress)
                Main.Panel.SetProgress(__instance.song.time / __instance.song.clip.length);
        }
    }
}
