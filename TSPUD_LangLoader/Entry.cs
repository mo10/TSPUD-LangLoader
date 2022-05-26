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

namespace TSPUD_LangLoader
{
    class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            Application.runInBackground = true;

            AssetManager.Init();
            SetFallbackFont();

            HarmonyInstance.PatchAll();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void SetFallbackFont()
        {
            try
            {
                Font font = AssetManager.Get<Font>("SourceHanSans");
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);
                if (fontAsset)
                {
                    fontAsset.name = $"Patched {font.name}";
                    TMP_Settings.fallbackFontAssets.Add(fontAsset);
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Unable to load font", ex);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
#if DEBUG
            MelonLogger.Msg($"Scene loaded: {scene.name}");
#endif
            using (_ = new Diagnosis("ScenePatch"))
                foreach (var texture in textures)
                {
                    try
                    {
                        var new_texture = AssetManager.Get<Texture2D>(texture.name);
                        if (new_texture == null)
                            continue;

                        texture.UpdateExternalTexture(new_texture.GetNativeTexturePtr());
                        texture.name = $"Patched {texture.name}";
#if DEBUG
                        MelonLogger.Msg($"Patched texture: {texture.name}");
#endif
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error($"Failed to patch texture: {texture.name}", ex);
                    }
                }
        }

        public override void OnUpdate()
        {
            if (StanleyController.Instance == null)
                return;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StanleyController.Instance.FieldOfView = 10;
                MainCamera.Camera.fieldOfView = (MainCamera.Camera.fieldOfView / 2);
                MelonLogger.Msg("Zoom in");
            }
            if (Input.GetKeyUp(KeyCode.Z))
            {
                StanleyController.Instance.FieldOfView = StanleyController.Instance.FieldOfViewBase;
                MelonLogger.Msg("Zoom out");
            }
        }
    }


    [HarmonyPatch]
    static class LanguagePatch
    {
        private static bool IsLoaded = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), "RegisterSourceInResources")]
        public static void RegisterSourceInResourcesPrefix()
        {
            if (IsLoaded) return;

            foreach (string name in LocalizationManager.GlobalSources)
            {
                LanguageSourceAsset asset = ResourceManager.pInstance.GetAsset<LanguageSourceAsset>(name);
                if (asset && !LocalizationManager.Sources.Contains(asset.mSource))
                {
                    IsLoaded = true;

                    // Add custom fonts to game asset manager
                    asset.mSource.AddAsset(AssetManager.Get<Font>("SourceHanSans"));
                    // Import translate
                    string filename = Path.Combine(MelonUtils.UserDataDirectory, $"translate.csv");
                    string text = AssetManager.GetText("translate");
                    if (string.IsNullOrEmpty(text))
                    {
                        MelonLogger.Msg($"Translate exported: {filename}");
                        File.WriteAllText(filename, asset.mSource.Export_CSV("", specializationsAsRows: false));
                    }
                    else
                    {
                        asset.mSource.Import_CSV("", text, eSpreadsheetUpdateMode.Merge);
                        MelonLogger.Msg($"Translate loaded");
                    }
                }
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SequelTools), "DoStandardSequelReplacementStep")]
        public static bool DoStandardSequelReplacementStepPrefix(string originalText, IntConfigurable sequelCountConfigurable,
                            IntConfigurable prefixIndexConfigurable, IntConfigurable postfixIndexConfigurable, ref string __result)
        {
            int intValue = prefixIndexConfigurable.GetIntValue();
            string newValue = "";
            if (intValue != -1)
            {
                newValue = SequelTools.PrefixLocalizedText(intValue);
            }
            int intValue2 = postfixIndexConfigurable.GetIntValue();
            string newValue2 = "";
            if (intValue2 != -1)
            {
                newValue2 = SequelTools.PostfixLocalizedText(intValue2);
            }

            if (newValue.Contains("…"))
            {
#if DEBUG
                MelonLogger.Msg("replace placeholder");
#endif
                newValue = newValue.Replace("…", newValue2);
                newValue2 = "";
            }

            __result = originalText.Replace("\\n", "\n").Replace("%!N!%", sequelCountConfigurable.
                GetIntValue().ToString()).Replace("%!P!%", newValue).Replace(" %!S!%", newValue2);
#if DEBUG
            MelonLogger.Msg("final result {0}", __result);
#endif
            return false;
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
