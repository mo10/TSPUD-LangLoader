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

        float zoom_step = 6f;
        float zoom_stop = 0f;
        bool zoom_down = false;
        float default_mouse_sensX;
        float default_mouse_sensY;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoInlining)]
        void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            // Canvas
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoInlining)]
        void Update()
        {
            if (StanleyController.Instance == null)
                return;

            // 按Z缩放
            if (Input.GetKeyDown(KeyCode.Z))
            {
                zoom_down = true;
                zoom_stop = StanleyController.Instance.FieldOfViewBase / 2f;
                default_mouse_sensX = StanleyController.Instance.mouseSensitivityX;
                default_mouse_sensY = StanleyController.Instance.mouseSensitivityY;
                StanleyController.Instance.mouseSensitivityX = default_mouse_sensX * 0.5f;
                StanleyController.Instance.mouseSensitivityY = default_mouse_sensY * 0.5f;
                StanleyController.Instance.SetMovementSpeedMultiplier(0.5f);
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                zoom_down = false;
                zoom_stop = StanleyController.Instance.FieldOfViewBase;
                StanleyController.Instance.mouseSensitivityX = default_mouse_sensX;
                StanleyController.Instance.mouseSensitivityY = default_mouse_sensY;
                StanleyController.Instance.SetMovementSpeedMultiplier(1.0f);
            }

            if (zoom_down && StanleyController.Instance.FieldOfView >= zoom_stop)
            {
                StanleyController.Instance.FieldOfView -= zoom_step;
            }
            else if (!zoom_down && StanleyController.Instance.FieldOfView < zoom_stop)
            {
                StanleyController.Instance.FieldOfView += zoom_step;
            }
#if DEBUG
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                StanleyController.Instance.SetMovementSpeedMultiplier(3.0f);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                StanleyController.Instance.SetMovementSpeedMultiplier(1.0f);
            }
#endif
        }
    }
}
