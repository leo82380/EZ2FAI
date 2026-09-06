using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using static UnityModManagerNet.UnityModManager.ModEntry;

namespace EZ2FAI
{
    public static class Main
    {
        public static ModEntry Mod { get; private set; }
        public static ModLogger Logger { get; private set; }
        public static Harmony Harmony { get; private set; }
        public static Settings Settings { get; private set; }
        public static EZ2FAIPanel Panel { get; private set; }
        // Runtime-created profile image assets (from LoadImage + Sprite.Create).
        // Tracked so we can destroy the previous ones when the image changes,
        // otherwise repeated changes leak native texture/sprite memory.
        private static Sprite runtimeProfileSprite;
        private static Texture2D runtimeProfileTexture;
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
        }
        public static bool OnToggle(ModEntry modEntry, bool toggle)
        {
            if (toggle)
            {
                Settings = ModSettings.Load<Settings>(modEntry);
                Panel = EZ2FAIPanel.CreatePanel();
                Panel.Apply(Settings.Position, Settings.Scale);
                Panel.SetNickname(Settings.Username);
                SetProfileImage();
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                UnityEngine.Object.Destroy(Panel.gameObject);
                Panel = null;
                DestroyRuntimeProfile();
                Harmony.UnpatchAll(Harmony.Id);
                Harmony = null;
            }
            return true;
        }
        public static void OnGUI(ModEntry modEntry)
        {
            bool changed = false;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>Username</b>");
                string name = GUILayout.TextField(Settings.Username);
                if (name != Settings.Username)
                {
                    Settings.Username = name;
                    Panel.SetNickname(name);
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>Profile Image</b>");
                string image = GUILayout.TextField(Settings.ProfileImage);
                if (image != Settings.ProfileImage)
                {
                    Settings.ProfileImage = image;
                    SetProfileImage();
                }

                if (GUILayout.Button("Choose..."))
                {
                    string picked = UnityFileDialog.FileBrowser.PickFile(
                        filterName: "Images (jpg/jpeg/png)",
                        filterExtensions: new[] { "jpg", "jpeg", "png" },
                        title: "Select Profile Image");
                    if (!string.IsNullOrEmpty(picked))
                    {
                        Settings.ProfileImage = picked;
                        SetProfileImage();
                    }
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Label("<b>Position</b>");
            changed |= DrawVector2(ref Settings.Position);
            GUILayout.Label("<b>Scale</b>");
            changed |= DrawVector2(ref Settings.Scale);
            GUILayout.Label("<b>Pixel Per Unit</b>");
            changed |= DrawFloat("", ref Settings.pixelsPerUnitMultiplier, 1f, 4f);
            GUILayout.Label("<b>Title Font Size</b>");
            if (DrawFloat("", ref Settings.TitleFontSize, 0.5f, 3f)) Panel.ApplyFontSize();
            GUILayout.Label("<b>Value Font Size</b>");
            if (DrawFloat("", ref Settings.ValueFontSize, 0.5f, 3f)) Panel.ApplyFontSize();
            GUILayout.Label("<b>Panel Opacity</b>");
            if (DrawFloat("", ref Settings.PanelOpacity, 0.1f, 1f)) Panel.ApplyOpacity();
            GUILayout.Label("<b>Title Marquee Speed</b>");
            DrawFloat("", ref Settings.MarqueeSpeed, 10f, 300f);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Reset Position"))
                {
                    Settings.Position = new Vector2(0.16f, 0.1f);
                    Settings.Scale = new Vector2(0.7f, 0.7f);
                    Panel.Apply(Settings.Position, Settings.Scale);
                }
                if (GUILayout.Button("Reset Fonts / Opacity"))
                {
                    Settings.TitleFontSize = 1.5f;
                    Settings.ValueFontSize = 1.5f;
                    Settings.PanelOpacity = 1f;
                    Panel.ApplyFontSize();
                    Panel.ApplyOpacity();
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            if (changed) Panel.Apply(Settings.Position, Settings.Scale);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>Song Progress</b>");
                Settings.SongProgress = GUILayout.Toggle(Settings.SongProgress, "");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("<b>Drag Enabled</b>");
                Settings.DragEnabled = GUILayout.Toggle(Settings.DragEnabled, "");
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            // GUILayout.BeginHorizontal();
            // {
            //     GUILayout.Label("<b>Check Point</b>");
            //     Settings.IsCheckPoint = GUILayout.Toggle(Settings.IsCheckPoint, "");
            //     GUILayout.FlexibleSpace();
            // }
            // GUILayout.EndHorizontal();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            ModSettings.Save(Settings, modEntry);
        }
        public static void SetProfileImage()
        {
            if (Panel == null) return;
            if (string.IsNullOrEmpty(Settings.ProfileImage) || !File.Exists(Settings.ProfileImage))
            {
                Panel.SetProfileImage(null);
                DestroyRuntimeProfile();
                return;
            }
            Texture2D texture = new Texture2D(1, 1);
            try
            {
                var method = typeof(ImageConversion).GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) });
                if (method == null)
                {
                    UnityEngine.Object.Destroy(texture);
                    Panel.SetProfileImage(null);
                    DestroyRuntimeProfile();
                    return;
                }
                var bytes = File.ReadAllBytes(Settings.ProfileImage);
                bool ok = (bool)method.Invoke(null, new object[] { texture, bytes, false });
                if (!ok || texture.width <= 1)
                {
                    UnityEngine.Object.Destroy(texture);
                    Panel.SetProfileImage(null);
                    DestroyRuntimeProfile();
                    return;
                }
                var result = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                // Assign the new sprite first, then destroy the previous runtime assets.
                Panel.SetProfileImage(result);
                DestroyRuntimeProfile();
                runtimeProfileSprite = result;
                runtimeProfileTexture = texture;
            }
            catch
            {
                UnityEngine.Object.Destroy(texture);
                Panel.SetProfileImage(null);
                DestroyRuntimeProfile();
            }
        }
        private static void DestroyRuntimeProfile()
        {
            if (runtimeProfileSprite != null)
            {
                UnityEngine.Object.Destroy(runtimeProfileSprite);
                runtimeProfileSprite = null;
            }
            if (runtimeProfileTexture != null)
            {
                UnityEngine.Object.Destroy(runtimeProfileTexture);
                runtimeProfileTexture = null;
            }
        }
        public static bool DrawVector2(ref Vector2 vec2)
        {
            bool result = false;
            result |= DrawFloat("X:", ref vec2.x);
            result |= DrawFloat("Y:", ref vec2.y);
            return result;
        }
        public static bool DrawFloat(string label, ref float f, float leftValue = 0, float rightValue = 1)
        {
            GUILayout.BeginHorizontal();
            float newValue = NamedSliderContent(label, f, leftValue, rightValue, 300);
            GUILayout.EndHorizontal();
            bool result = newValue != f;
            f = newValue;
            return result;
        }
        public static float NamedSliderContent(
            string name,
            float value,
            float leftValue,
            float rightValue,
            float sliderWidth,
            float roundNearest = 0,
            float labelWidth = 0,
            string valueFormat = "{0}")
        {
            if (labelWidth == 0)
            {
                GUILayout.Label(name, GUILayout.Width(100f));
                GUILayout.Space(4f);
            }
            else
            {
                GUILayout.Label(name, GUILayout.Width(labelWidth));
            }
            float newValue =
                GUILayout.HorizontalSlider(
                    value, leftValue, rightValue, GUILayout.Width(sliderWidth));
            if (roundNearest != 0)
            {
                newValue = Mathf.Round(newValue / roundNearest) * roundNearest;
            }
            GUILayout.Space(8f);
            if (valueFormat != "{0}")
                GUILayout.Label(string.Format(valueFormat, newValue));
            else
            {
                // TryParse writes 0 into its out param on failure, so parse into a
                // temp and only commit on success — otherwise clearing/mid-editing
                // the field would snap the slider to 0.
                string text = GUILayout.TextField(newValue.ToString("F4"));
                if (float.TryParse(text, out float parsed))
                    newValue = parsed;
            }
            GUILayout.FlexibleSpace();
            return newValue;
        }
    }
}
