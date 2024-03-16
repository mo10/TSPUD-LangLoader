using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using I2.Loc;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MelonLoader.Utils;
using Entrypoint;

namespace TSPUD_LangLoader
{
    class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            Application.runInBackground = true;

            Entrypoint.Patch.DoPathcing();
            HarmonyInstance.PatchAll();
        }
    }

    [HarmonyPatch]
    static class BuildNumberPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SetBuildNumberText), "UpdateText")]
        public static void UpdateTextPostfix(ref TMP_Text ___text)
        {
            ___text.text = $"<size=200%>起源汉化组</size>\n" +
#if BUNDLE
                "打包" +
#else
                "散装" +
#endif

#if DEBUG
                $"测试版\n" +
#else
                $"发布版\n" +
#endif
                (Application.version ?? "未知版本");
        }
    }
}
