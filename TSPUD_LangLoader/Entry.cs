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

namespace TSPUD_LangLoader
{
    class Entry : MelonMod
    {
        public override void OnApplicationStart()
        {
            Application.runInBackground = true;
            HarmonyInstance.PatchAll();
        }
    }

    [HarmonyPatch]
    static class Patch
    {
        private static bool IsLoaded = false;

        private static Dictionary<string, TMP_FontAsset> LoadedFonts = new Dictionary<string, TMP_FontAsset>();

        private static Dictionary<string, string> FontMapping = new Dictionary<string, string>()
        {
            {"PalatinoLinotype-Roman_-_Palatino_Linotype_-_Regular SDF","SourceHanSans"},
            {"iosevka-medium SDF","SourceHanSans"},
            {"iosevka-extralight SDF","SourceHanSans"},
            {"iosevka-thin SDF","SourceHanSans"},
            {"Leauge_Gothic_Regular","SourceHanSans"},
            {"NotoSans-Regular SDF","SourceHanSans"},
            {"NotoSans-Bold SDF","SourceHanSans"},
            {"HKNova-Narrow SDF","SourceHanSans"},
        };
        private static Dictionary<string, string> MatMapping = new Dictionary<string, string>()
        {
            {"Leauge_Gothic_Regular - Button Prompt","SourceHanSans"},
            {"Leauge_Gothic_Regular - Hole Ending Long Shadow","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Option Hover","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Option Normal With Glow","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Option Normal","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Sequel Gold","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Sequel White","SourceHanSans"},
            {"Leauge_Gothic_Regular - Menu Subheader","SourceHanSans"},
            {"Leauge_Gothic_Regular - Museum","SourceHanSans"},
            {"Leauge_Gothic_Regular - No Dropshadow","SourceHanSans"},
            {"HKNova-Narrow - NCP2 Nav Plaque","SourceHanSans"},
            {"HKNova-Narrow - Object Label","SourceHanSans"},
            {"HKNova-Narrow - Option Glow","SourceHanSans"},
            {"HKNova-Narrow - Option Hover","SourceHanSans"},
            {"HKNova-Narrow - Option Normal","SourceHanSans"},
            {"HKNova-Narrow - Settings Label","SourceHanSans"},
            {"HKNova-Narrow - Subtitles","SourceHanSans"},
            {"HKNova-Narrow - Tooltip","SourceHanSans"},
            {"NotoSans-Regular SDF - NCP2 Nav Plaque","SourceHanSans"},
            {"NotoSans-Regular SDF - Object Label","SourceHanSans"},
            {"NotoSans-Regular SDF - Option Glow","SourceHanSans"},
            {"NotoSans-Regular SDF - Option Hover","SourceHanSans"},
            {"NotoSans-Regular SDF - Option Normal","SourceHanSans"},
            {"NotoSans-Regular SDF - Settings Label","SourceHanSans"},
            {"NotoSans-Regular SDF - Shadow","SourceHanSans"},
            {"NotoSans-Regular SDF - Subtitles","SourceHanSans"},
            {"NotoSans-Regular SDF - Tooltip","SourceHanSans"},
        };

        private static Material SharedMaterial = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field"));

        static Patch()
        {
            LoadedFonts = LoadAllFonts();
            TMP_Settings.fallbackFontAssets.AddRange(LoadedFonts.Values.ToList());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LocalizationManager), "RegisterSourceInResources")]
        public static void RegisterSourceInResourcesPrefix()
        {
            if (IsLoaded)
                return;

            foreach (string name in LocalizationManager.GlobalSources)
            {
                LanguageSourceAsset asset = ResourceManager.pInstance.GetAsset<LanguageSourceAsset>(name);
                if (asset && !LocalizationManager.Sources.Contains(asset.mSource))
                {
                    IsLoaded = true;

                    foreach (var fontAsset in LoadedFonts)
                    {
                        asset.mSource.AddAsset(fontAsset.Value);
                        MelonLogger.Msg($"Loaded: {fontAsset.Key}");
                    }

                    //foreach (var pair in FontMapping)
                    //{
                    //    var oldAsset = asset.mSource.Assets.Find(a => a.name == pair.Key);
                    //    var newAsset = asset.mSource.Assets.Find(a => a.name == pair.Value);

                    //    var clone = UnityEngine.Object.Instantiate(newAsset);
                    //    clone.name = oldAsset.name;
                    //    asset.mSource.Assets.Remove(oldAsset);
                    //    asset.mSource.AddAsset(clone);
                    //}

                    string filename = Path.Combine(MelonUtils.UserDataDirectory, $"translate.csv");

                    if (File.Exists(filename)) {
                        MelonLogger.Msg($"Loaded: {filename}");
                        var raw_csv = File.ReadAllText(filename);
                        asset.mSource.Import_CSV("", raw_csv, eSpreadsheetUpdateMode.Merge);
                    }
                    else
                    {
                        MelonLogger.Msg($"Export translate to: {filename}");
                        File.WriteAllText(filename, asset.mSource.Export_CSV("", specializationsAsRows: false));
                    }
                }
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(TMP_Text),"set_text")]
        public static void TMP_TextPrefix(ref string value,  ref TMP_Text __instance)
        {
            if (FontMapping.TryGetValue(__instance.font.name,out var replace_name))
            {
                if (LoadedFonts.TryGetValue(replace_name, out var fontAsset))
                {
                    __instance.font = fontAsset;
                    __instance.material = SharedMaterial;
                    __instance.fontMaterial = SharedMaterial;
                    __instance.fontMaterials = new Material[] { SharedMaterial };
                    __instance.fontSharedMaterial = SharedMaterial;
                    __instance.fontSharedMaterials = new Material[] { SharedMaterial };

                    __instance.SetMaterialDirty();

                    MelonLogger.Msg($"TMP_Text value:{value} font: {__instance.font.name} material: {__instance.fontMaterial.name}");

                }
                else
                {
                    MelonLogger.Msg($"Font not found: {replace_name}");
                }
            }
        }

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(TMP_Text), "set_fontSharedMaterial")]
        public static void set_fontSharedMaterial(ref Material value, ref TMP_Text __instance)
        {

            if(__instance.font.name == "SourceHanSans")
            {
                if (MatMapping.ContainsKey(value.name))
                    value = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field")) { mainTexture = new Texture2D(1, 1)};
                MelonLogger.Msg($"text: {__instance.text} Fixed material");
            }

            if (FontMapping.TryGetValue(__instance.font.name, out var replace_name))
            {
                if (LoadedFonts.TryGetValue(replace_name, out var fontAsset))
                {
                    __instance.font = fontAsset;
                    value = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field")) { mainTexture = new Texture2D(1, 1) };
                    MelonLogger.Msg($"New font: {__instance.font.name} material: {__instance.fontMaterial.name}");
                }
                else
                {
                    MelonLogger.Msg($"Font not found: {replace_name}");
                }
            }
        }

        private static Dictionary<string, TMP_FontAsset> LoadAllFonts()
        {
            Dictionary<string, TMP_FontAsset> fontAssets = new Dictionary<string, TMP_FontAsset>();

            string searchPath = Path.Combine(MelonUtils.UserDataDirectory, $"fonts");
            foreach(var filePath in Directory.GetFiles(searchPath, "*.ttf"))
            {
                Font font = new Font(filePath);
                TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);
                if (fontAsset)
                {
                    fontAsset.name = Path.GetFileNameWithoutExtension(filePath);
                    fontAssets.Add(fontAsset.name ,fontAsset);
                }
            }
            return fontAssets;
        }
    }
}
