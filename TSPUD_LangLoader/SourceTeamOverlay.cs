using MelonLoader;
using Steamworks;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace TSPUD_LangLoader
{
    class SourceTeamOverlay : MonoBehaviour
    {
        public Text textTL;
        public Text textTC;
        public Text textTR;
        public Text textBL;
        public Text textBC;
        public Text textBR;
        private bool isInited=false;
        private bool isSteamInited=false;
        private static SourceTeamOverlay m_instance;

        public static SourceTeamOverlay Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameObject("SourceTeamOverlay").AddComponent<SourceTeamOverlay>();
                    m_instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(m_instance.gameObject);
                }
                return m_instance;
            }
        }

        public void setup()
        {
            if (isInited)
                return;

            isInited = true;
            var canvas = gameObject.AddComponent<Canvas>();
            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            var graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

            // Canvas
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            textTL = CreateText(new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), TextAnchor.UpperLeft);
            textTC = CreateText(new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), TextAnchor.UpperCenter);
            textTR = CreateText(new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), TextAnchor.UpperRight);

            textBL = CreateText(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), TextAnchor.LowerLeft);
            textBC = CreateText(new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), TextAnchor.LowerCenter);
            textBR = CreateText(new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), TextAnchor.LowerRight);

            MelonLogger.Msg("Overlay loaded");
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        void Update()
        {
            if (SteamManager.Initialized &&　!isSteamInited)
            {
                isSteamInited = true;
                textBC.text = SteamUser.GetSteamID().ToString();
            }
        }

        Text CreateText(Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, TextAnchor textAnchor)
        {
            var textGameObject = new GameObject("Watermark");
            DontDestroyOnLoad(textGameObject);
            textGameObject.transform.parent = gameObject.transform;

            var text = textGameObject.AddComponent<Text>();
            text.font = AssetManager.Get<Font>("SourceHanSans");
            text.color = new Color(1, 1, 1, 0.2f);
            text.text = $"起源汉化组";
            text.fontSize = 30;
            text.alignment = textAnchor;

            var rect = textGameObject.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(0, 0, 0);
            rect.sizeDelta = new Vector2(350, 150);
            rect.pivot = pivot;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;

            return text;
        }
    }
}
