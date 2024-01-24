using ADOFAI;
using System;
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
        public void SetNickname(string nickName)
        {
            nickText.text = nickName;
            if (nickName.Contains("섹") || nickName.Contains("Sex", StringComparison.OrdinalIgnoreCase) ||
                nickName.Contains("Suck", StringComparison.OrdinalIgnoreCase))
            {
                nickText.color = Color.white;
                nickText.enableVertexGradient = true;
                nickText.colorGradient =
                    new VertexGradient(
                        new Color(1, 1, 0),
                        new Color(0, 1, 1),
                        new Color(1, 0, 1),
                        new Color(0.5f, 1, 0.5f)
                        );
            }
            else
            {
                nickText.color = Color.white;
                nickText.colorGradient = new VertexGradient(Color.white);
                nickText.enableVertexGradient = false;
            }
        }
        public void SetJudgeAccuracy(scrController ctrl)
        {
            for (int i = 0; i < 7; i++)
                judgeCountTexts[i].text = ctrl.mistakesManager.GetHits((HitMargin)i).ToString();
            judgePercentText.text = Math.Round(ctrl.mistakesManager.percentXAcc * 100, 2) + "%";
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
            mapNameText.text = artist + " - " + song;
            Main.Logger.Log($"authorText:{authorText.text}, mapNameText{mapNameText.text}");
        }
        public void SetProfileImage(Sprite sprite)
        {
            profileImage.sprite = sprite;
        }
        public void ResetJudgeAccuracy()
        {
            for (int i = 0; i < 7; i++)
                judgeCountTexts[i].text = "0";
            judgePercentText.text = "0%";
        }
        public void ResetProgress()
        {
            progressInner.fillAmount = 0;
        }
        public void ResetMapName()
        {
            mapNameText.text = "나는 석큐버스를";
            authorText.text = "사랑해요";
        }
        public void Apply(Vector2 position, Vector2 scale)
        {
            var t = background.transform;
            t.localPosition = new Vector2(position.x * Screen.width - Screen.width / 2, position.y * Screen.height - Screen.height / 2);
            t.localScale = scale;
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
            ResetMapName();
            ResetJudgeAccuracy();
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
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
