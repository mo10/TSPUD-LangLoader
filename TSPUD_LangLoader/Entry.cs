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

            TextureManager.Init();
            HarmonyInstance.PatchAll();

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var texs = Resources.FindObjectsOfTypeAll<Texture2D>();
#if DEBUG
            MelonLogger.Msg($"Scene loaded: {scene.name} {texs.Length}");
#endif
            foreach (var tex in texs)
            {
                TextureManager.PatchTexture(tex);
            }
        }
    }

    [HarmonyPatch]
    static class LanguagePatch
    {
        private static bool IsLoaded = false;

        static LanguagePatch()
        {
            // Setup TMP fallback fonts
            FontManager.Init();
            TMP_Settings.fallbackFontAssets.AddRange(FontManager.Fonts.Values.ToList());
        }

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
                    foreach (var fontAsset in FontManager.Fonts)
                    {
                        asset.mSource.AddAsset(fontAsset.Value);
                    }

                    string filename = Path.Combine(MelonUtils.UserDataDirectory, $"translate.csv");

                    if (File.Exists(filename))
                    {
#if DEBUG
                        MelonLogger.Msg($"Translate loaded: {filename}");
#endif
                        var raw_csv = File.ReadAllText(filename);
                        asset.mSource.Import_CSV("", raw_csv, eSpreadsheetUpdateMode.Merge);
                    }
                    else
                    {
#if DEBUG
                        MelonLogger.Msg($"Translate exported: {filename}");
#endif
                        File.WriteAllText(filename, asset.mSource.Export_CSV("", specializationsAsRows: false));
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
                MelonLogger.Msg("replace placeholder");
                newValue = newValue.Replace("…", newValue2);
                newValue2 = "";
            }

            __result = originalText.Replace("\\n", "\n").Replace("%!N!%", sequelCountConfigurable.
                GetIntValue().ToString()).Replace("%!P!%", newValue).Replace(" %!S!%", newValue2);

            MelonLogger.Msg("final result {0}", __result);
            return false;
        }
    }
}
