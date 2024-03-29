﻿using HarmonyLib;
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
        public static Sprite Suckyoubus { get; private set; }
        public static void Load(ModEntry modEntry)
        {
            Mod = modEntry;
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(Mod.Path, "suckyoubus.png")));
            Suckyoubus = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
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
                Harmony.UnpatchAll(Harmony.Id);
                Harmony = null;
            }
            return true;
        }
        public static void OnGUI(ModEntry modEntry)
        {
            bool changed = false;
            GUILayout.BeginHorizontal();
            ButtonLabel("<b>Username</b>", OpenDiscordUrl);
            string name = GUILayout.TextField(Settings.Username);
            if (name != Settings.Username)
            {
                Settings.Username = name;
                Panel.SetNickname(name);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            ButtonLabel("<b>Profile Image</b>", OpenDiscordUrl);
            string image = GUILayout.TextField(Settings.ProfileImage);
            if (image != Settings.ProfileImage)
            {
                Settings.ProfileImage = image;
                SetProfileImage();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            ButtonLabel("<b>Position</b>", OpenDiscordUrl);
            changed |= DrawVector2(ref Settings.Position);
            ButtonLabel("<b>Scale</b>", OpenDiscordUrl);
            changed |= DrawVector2(ref Settings.Scale);
            if (changed) Panel.Apply(Settings.Position, Settings.Scale);

            GUILayout.BeginHorizontal();
            ButtonLabel("<b>Song Progress</b>", OpenDiscordUrl);
            Settings.SongProgress = GUILayout.Toggle(Settings.SongProgress, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ButtonLabel("<b>Drag Enabled</b>", OpenDiscordUrl);
            Settings.DragEnabled = GUILayout.Toggle(Settings.DragEnabled, "");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        public static void OnSaveGUI(ModEntry modEntry)
        {
            ModSettings.Save(Settings, modEntry);
        }
        public static void SetProfileImage()
        {
            if (string.IsNullOrEmpty(Settings.ProfileImage))
                Panel.SetProfileImage(null);
            else if (Settings.ProfileImage.Equals("Suckyoubus", StringComparison.OrdinalIgnoreCase))
                Panel.SetProfileImage(Suckyoubus);
            else if (File.Exists(Settings.ProfileImage))
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(File.ReadAllBytes(Settings.ProfileImage));
                var result = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
                Panel.SetProfileImage(result);
            }
        }
        public static bool DrawVector2(ref Vector2 vec2)
        {
            bool result = false;
            result |= DrawFloat("X:", ref vec2.x);
            result |= DrawFloat("Y:", ref vec2.y);
            return result;
        }
        public static bool DrawFloat(string label, ref float f)
        {
            GUILayout.BeginHorizontal();
            float newValue = NamedSliderContent(label, f, 0, 1, 300);
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
                ButtonLabel(name, OpenDiscordUrl);
                GUILayout.Space(4f);
            }
            else
            {
                ButtonLabel(name, OpenDiscordUrl, GUILayout.Width(labelWidth));
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
                ButtonLabel(string.Format(valueFormat, newValue), OpenDiscordUrl);
            else float.TryParse(GUILayout.TextField(newValue.ToString("F4")), out newValue);
            GUILayout.FlexibleSpace();
            return newValue;
        }
        public static void ButtonLabel(string label, Action onPressed, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(label, GUI.skin.label, options))
                onPressed?.Invoke();
        }
        public static void OpenDiscordUrl()
        {
            //Application.OpenURL("https://discord.gg/tadjC4DyTn");
        }
        static string[] verbs = new string[]
        {
            "좋아해요",
            "사랑해요",
            "연모해요",
            "흠모해요",
            "사모해요",
            "귀여워해요",
            "존경해요",
            "이뻐해요"
        };
        static string[] verbs2 = new string[]
        {
            "다이스키~♥",
            "아이시떼루~♥",
        };
        public static string GetRandomVerb()
        {
            return verbs[(int)Math.Round(UnityEngine.Random.value * (verbs.Length - 1))];
        }
        public static string GetRandomVerb2()
        {
            return verbs2[(int)Math.Round(UnityEngine.Random.value * (verbs2.Length - 1))];
        }
        public static void EnableRainbow(TextMeshProUGUI text, VertexGradient? grad = null)
        {
            text.color = Color.white;
            text.enableVertexGradient = true;
            text.colorGradient = grad ??
                new VertexGradient(
                    new Color(1, 1, 0),
                    new Color(0, 1, 1),
                    new Color(1, 0, 1),
                    new Color(0.5f, 1, 0.5f)
                    );
        }
        public static void DisableRainbow(TextMeshProUGUI text)
        {
            text.color = Color.white;
            text.colorGradient = new VertexGradient(Color.white);
            text.enableVertexGradient = false;
        }
    }
}
