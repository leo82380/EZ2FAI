using System.Text.RegularExpressions;
using ADOFAI;
using HarmonyLib;

[HarmonyPatch(typeof(LevelData),"LoadLevel")]
public static class Levelload
{
    public static void Postfix(LevelData __instance)
    {
        Main.level_author = DeleteRichTag(__instance.author);
        Main.level_artist = DeleteRichTag(__instance.artist);
        Main.level_song = DeleteRichTag(__instance.song);
        Main.level_songtist = Main.level_artist + " - " + Main.level_song;

        Main.autherTxt.text = Main.level_author;
        Main.songtistTxt.text = Main.level_songtist;

    }
    public static readonly Regex DelTag = new Regex(@"<(color|material|quad|size)=(.|\n)*?>|<\/(color|material|quad|size)>|<(b|i)>|<\/(b|i)>",RegexOptions.Compiled | RegexOptions.Multiline);
    public static string DeleteRichTag(this string s)
        => DelTag.Replace(s,string.Empty);
}