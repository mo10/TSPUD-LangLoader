using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Entrypoint
{
    class SourceTeamOverlay : MonoBehaviour
    {
        public Text textTL;
        public Text textTC;
        public Text textTR;
        public Text textBL;
        public Text textBC;
        public Text textBR;
        private bool isSteamInited = false;
        float zoom_step = 5f;
        float zoom_stop = 0f;
        bool zoom_down = false;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoInlining)]
        void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            //var graphicRaycaster = gameObject.AddComponent<GraphicRaycaster>();

            // Canvas
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            textTL = CreateText(new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), TextAnchor.UpperLeft);
            textTC = CreateText(new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), TextAnchor.UpperCenter);
            textTR = CreateText(new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), TextAnchor.UpperRight);

            textBL = CreateText(new Vector2(0, 0), new Vector2(0, 0), new Vector2(0, 0), TextAnchor.LowerLeft);
            textBC = CreateText(new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0), TextAnchor.LowerCenter);
            textBR = CreateText(new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0), TextAnchor.LowerRight);

            Logger.Debug("Awake");
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoInlining)]
        void Update()
        {
            if (SteamManager.Initialized && !isSteamInited)
            {
                isSteamInited = true;
                textBC.text = SteamUser.GetSteamID().ToString();
                Logger.Debug("Update");
            }
            if (StanleyController.Instance == null)
                return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                zoom_down = true;
                zoom_stop = StanleyController.Instance.FieldOfViewBase / 2;
                //StanleyController.Instance.FieldOfView = 10;

                //MainCamera.Camera.fieldOfView = (MainCamera.Camera.fieldOfView / 2);
                //Logger.Debug("Zoom in");
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                zoom_down = false;
                zoom_stop = StanleyController.Instance.FieldOfViewBase;
                //Logger.Debug("Zoom out");
            }
            if(zoom_down && StanleyController.Instance.FieldOfView >= zoom_stop)
            {
                StanleyController.Instance.FieldOfView -= zoom_step;
            }
            else if (!zoom_down && StanleyController.Instance.FieldOfView < zoom_stop)
            {
                StanleyController.Instance.FieldOfView += zoom_step;
            }
            //else if (zoom_stop !=0 && StanleyController.Instance.FieldOfView != zoom_stop)
            //{
            //    StanleyController.Instance.FieldOfView = zoom_stop;
            //    zoom_stop = 0;
            //}
        }

        Text CreateText(Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax, TextAnchor textAnchor)
        {
            var textGameObject = new GameObject("Watermark");
            DontDestroyOnLoad(textGameObject);
            textGameObject.transform.parent = gameObject.transform;

            var text = textGameObject.AddComponent<Text>();
            text.font = AssetManager.Get<Font>("SourceHanSans");
            text.color = new Color(1, 1, 1, 0.2f);
            text.text = $"起源汉化组内部测试";
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
