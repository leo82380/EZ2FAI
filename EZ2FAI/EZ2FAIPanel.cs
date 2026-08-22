using ADOFAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EZ2FAI
{
    public class EZ2FAIPanel : MonoBehaviour, IDragHandler
    {
        public Image profileImage;
        public Image profileImageMask;
        public Image background;
        public Image progressOuter;
        public Image progressInner;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI nickText;
        public TextMeshProUGUI mapNameText;
        public TextMeshProUGUI authorText;
        public TextMeshProUGUI judgeTitleText;
        public TextMeshProUGUI judgePercentText;
        public TextMeshProUGUI[] judgeTitleTexts;
        public TextMeshProUGUI[] judgeCountTexts;
        public TextMeshProUGUI curBPMTitleText;
        public TextMeshProUGUI curBPMText;
        public TextMeshProUGUI realBPMTitleText;
        public TextMeshProUGUI realBPMText;
        private readonly List<TextMeshProUGUI> titleTexts = new List<TextMeshProUGUI>();
        private readonly List<float> titleBaseSizes = new List<float>();
        private readonly List<TextMeshProUGUI> valueTexts = new List<TextMeshProUGUI>();
        private readonly List<float> valueBaseSizes = new List<float>();
        public void SetNickname(string nickName)
        {
            nickText.text = nickName;
        }

        public void SetJudgeAccuracy(scrPlayer player)
        {
            var mistakeTracker = player.marginTracker;
            if (judgeCountTexts != null)
                for (int i = 0; i < 7 && i < judgeCountTexts.Length; i++)
                    if (judgeCountTexts[i] != null)
                        judgeCountTexts[i].text = mistakeTracker.GetHits((HitMargin)i).ToString();
            judgePercentText.text = Math.Round(player.marginTracker.percentXAcc * 100, 2) + "%";
        }

        public void SetProgress(float fillAmount)
        {
            progressInner.fillAmount = fillAmount;
        }
        public void SetMapName(LevelData data)
        {
            string author = RichTagBreaker.Replace(data.author, string.Empty);
            string artist = RichTagBreaker.Replace(data.artist, string.Empty);
            string song = RichTagBreaker.Replace(data.song, string.Empty);
            authorText.text = "BY " + author;
            string title = artist + " - " + song;
            if (song.Length > 7 || artist.Length > 5)
                title =
                    (artist.Length > 5 ? artist.Substring(0, 5) + "..." : artist) +
                    " - " +
                    (song.Length > 7 ? song.Substring(0, 7) + "..." : song);
            mapNameText.text = title;
        }
        public void SetProfileImage(Sprite sprite)
        {
            profileImage.sprite = sprite;
        }
        public void ResetJudgeAccuracy()
        {
            if (judgeCountTexts != null)
                for (int i = 0; i < 7 && i < judgeCountTexts.Length; i++)
                    if (judgeCountTexts[i] != null)
                        judgeCountTexts[i].text = "0";
            judgePercentText.text = "0%";
        }
        public void ResetProgress()
        {
            progressInner.fillAmount = 0;
        }
        public void ResetMapName()
        {
            mapNameText.text = "";
            authorText.text = "";
        }
        public void Apply(Vector2 position, Vector2 scale)
        {
            var t = background.transform;
            t.localPosition = new Vector2(position.x * Screen.width - Screen.width / 2, position.y * Screen.height - Screen.height / 2);
            t.localScale = scale;
            background.pixelsPerUnitMultiplier = Main.Settings.pixelsPerUnitMultiplier;
            ResetMapName();
        }
        public static EZ2FAIPanel CreatePanel()
        {
            if (MPCanvasPrefab != null)
                return Instantiate(MPCanvasPrefab).AddComponent<EZ2FAIPanel>();
            AssetBundle assets = AssetBundle.LoadFromFile(Path.Combine(Main.Mod.Path, "EZ2FAI.assets"));
            MPCanvasPrefab = assets.LoadAsset<GameObject>("MPCanvas 12");
            return Instantiate(MPCanvasPrefab).AddComponent<EZ2FAIPanel>();
        }
        
#pragma warning disable IDE0051
        private void Awake()
        {
            // AssetBundle By Leo82380
            DontDestroyOnLoad(gameObject);
            var bg = transform.Find("BackGround");
            var imgT = bg.Find("Image");
            background = bg.GetComponent<Image>();
            var maskT = bg.Find("ProfileImage"); // Mask
            profileImageMask = maskT.GetComponent<Image>();
            profileImage = maskT.Find("Image").GetComponent<Image>();
            progressOuter = imgT.GetComponent<Image>();
            progressInner = imgT.Find("Progress").GetComponent<Image>();
            nameText = bg.Find("Name").GetComponent<TextMeshProUGUI>();
            nickText = bg.Find("Nick").GetComponent<TextMeshProUGUI>();
            mapNameText = bg.Find("MapName").GetComponent<TextMeshProUGUI>();
            authorText = bg.Find("Author").GetComponent<TextMeshProUGUI>();
            var judgeRateT = bg.Find("JudgeRate");
            judgeTitleText = judgeRateT.GetComponent<TextMeshProUGUI>();
            judgePercentText = judgeRateT.Find("Percent").GetComponent<TextMeshProUGUI>();
            int count = 0;
            judgeTitleTexts = new TextMeshProUGUI[7];
            judgeCountTexts = new TextMeshProUGUI[7];
            foreach (Transform child in bg.Find("Judge"))
            {
                judgeTitleTexts[count] = child.GetComponent<TextMeshProUGUI>();
                judgeCountTexts[count] = child.Find("Count").GetComponent<TextMeshProUGUI>();
                count++;
            }
            var cbpm = bg.Find("CurBPM");
            curBPMTitleText = cbpm.GetComponent<TextMeshProUGUI>();
            curBPMText = cbpm.Find("CurBPMText").GetComponent<TextMeshProUGUI>();
            var rbpm = bg.Find("RealBPM");
            realBPMTitleText = rbpm.GetComponent<TextMeshProUGUI>();
            realBPMText = rbpm.Find("RealBPMText").GetComponent<TextMeshProUGUI>();
            judgeTitleText.text = "Accuracy";
            FixJudgeLayout(bg);
            FixMapNamePosition(bg);
            RegisterTexts();
            FixFonts();
            ApplyFontSize();
            ApplyOpacity();
            ResetMapName();
            ResetJudgeAccuracy();
        }

        // Panel transparency: fade the background artwork (and progress bar)
        // so the game shows through, while the text stays readable.
        public void ApplyOpacity()
        {
            float a = Main.Settings != null ? Mathf.Clamp01(Main.Settings.PanelOpacity) : 1f;
            if (background != null)
            {
                var c = background.color;
                c.a = a;
                background.color = c;
            }
            if (progressOuter != null)
            {
                var c = progressOuter.color;
                c.a = a;
                progressOuter.color = c;
            }
            if (progressInner != null)
            {
                var c = progressInner.color;
                c.a = a;
                progressInner.color = c;
            }
        }

        // v3 fix: keep the judge block exactly where it was designed (X and the
        // GridLayoutGroup rows untouched), and only shift it vertically so the
        // VL/TE/... labels sit on the same line as the "Accuracy" title. Done
        // in world space after the first layout frame, so it is correct no
        // matter the resolution or CanvasScaler scaling.
        private void FixJudgeLayout(Transform bg)
        {
            StartCoroutine(FixJudgeLayoutRoutine(bg));
        }

        private IEnumerator FixJudgeLayoutRoutine(Transform bg)
        {
            yield return null; // wait one frame for the canvas to be laid out
            try
            {
                var judgeRT = bg.Find("Judge") as RectTransform;
                var judgeRate = bg.Find("JudgeRate") as RectTransform;
                if (judgeRT == null || judgeRate == null) yield break;
                Vector3 judgePos = judgeRT.position;
                Vector3 accPos = judgeRate.position;
                // The rows are laid out from the block's top edge; align that
                // top edge with Accuracy's top edge so the labels share a line.
                float topHalf = judgeRT.rect.height * 0.5f * judgeRT.lossyScale.y;
                judgeRT.position = new Vector3(judgePos.x, accPos.y - topHalf, judgePos.z);
            }
            catch { }
        }

        // The map name / author sit too far right (right-anchored at -286);
        // nudge them left a bit for a nicer look.
        private void FixMapNamePosition(Transform bg)
        {
            try
            {
                var map = bg.Find("MapName") as RectTransform;
                var author = bg.Find("Author") as RectTransform;
                if (map != null)
                    map.anchoredPosition = new Vector2(-390f, map.anchoredPosition.y);
                if (author != null)
                    author.anchoredPosition = new Vector2(-390f, author.anchoredPosition.y);
            }
            catch { }
        }

        // ADOFAI v3 / Unity 6 fix:
        // EZ2FAI.assets was built with Unity 2022.3, and its bundled TMP font
        // ("SB agr M SDF" TMP_FontAsset) fails to deserialize on the Unity 6
        // game, leaving every TextMeshProUGUI with a null font. That throws a
        // NullReferenceException in TMPro.MaterialReference..ctor on every
        // canvas rebuild (spams the log every frame and can make the game
        // stutter). Swap in the game's own localized TMP font instead, which
        // is guaranteed to work because ADOFAI's own UI uses it.
        private void FixFonts()
        {
            try
            {
                TMP_FontAsset font = null;
                try
                {
                    RDString.Setup();
                    font = RDString.fontData.fontTMP;
                }
                catch { }
                if (font == null)
                    font = RDConstants.data.latinFontTMPro;
                if (font == null)
                    return;
                foreach (var t in new TextMeshProUGUI[] { nameText, nickText, mapNameText, authorText, judgeTitleText, judgePercentText, curBPMTitleText, curBPMText, realBPMTitleText, realBPMText })
                    if (t != null) t.font = font;
                for (int i = 0; i < 7; i++)
                {
                    if (judgeTitleTexts != null && judgeTitleTexts[i] != null) judgeTitleTexts[i].font = font;
                    if (judgeCountTexts != null && judgeCountTexts[i] != null) judgeCountTexts[i].font = font;
                }
            }
            catch { }
        }

        private void Register(TextMeshProUGUI t, bool isValue)
        {
            if (t == null) return;
            if (isValue) { valueTexts.Add(t); valueBaseSizes.Add(t.fontSize); }
            else { titleTexts.Add(t); titleBaseSizes.Add(t.fontSize); }
        }

        private void RegisterTexts()
        {
            // titles / labels
            Register(nameText, false);
            Register(nickText, false);
            Register(mapNameText, false);
            Register(authorText, false);
            Register(judgeTitleText, false);
            Register(curBPMTitleText, false);
            Register(realBPMTitleText, false);
            if (judgeTitleTexts != null)
                for (int i = 0; i < judgeTitleTexts.Length; i++) Register(judgeTitleTexts[i], false);
            // values / numbers
            Register(judgePercentText, true);
            Register(curBPMText, true);
            Register(realBPMText, true);
            if (judgeCountTexts != null)
                for (int i = 0; i < judgeCountTexts.Length; i++) Register(judgeCountTexts[i], true);
        }

        public void ApplyFontSize()
        {
            float titleScale = Main.Settings != null ? Main.Settings.TitleFontSize : 1f;
            float valueScale = Main.Settings != null ? Main.Settings.ValueFontSize : 1f;
            for (int i = 0; i < titleTexts.Count; i++)
            {
                var t = titleTexts[i];
                if (t == null) continue;
                t.enableAutoSizing = false;
                t.fontSize = titleBaseSizes[i] * titleScale;
            }
            for (int i = 0; i < valueTexts.Count; i++)
            {
                var t = valueTexts[i];
                if (t == null) continue;
                t.enableAutoSizing = false;
                t.fontSize = valueBaseSizes[i] * valueScale;
            }
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!Main.Settings.DragEnabled) return;
            Vector2 ePos = eventData.position;
            var x = ePos.x / Screen.width;
            var y = ePos.y / Screen.height;
            var position = new Vector2(x, y);
            Main.Settings.Position = position;
            Apply(position, Main.Settings.Scale);
        }
#pragma warning restore IDE0051
        private static GameObject MPCanvasPrefab;
        private static readonly Regex RichTagBreaker = new Regex(@"<(color|material|quad|size)=(.|\n)*?>|<\/(color|material|quad|size)>|<(b|i)>|<\/(b|i)>", RegexOptions.Compiled | RegexOptions.Multiline);
    }
}
