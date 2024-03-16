﻿using I2.Loc;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using HarmonyLib;
using System.Runtime.CompilerServices;
#if MELON
using MelonLoader;
using MelonLoader.Utils;
#endif

namespace Entrypoint
{
    static class Patch
    {
        public static void DoPathcing()
        {
            Logger.Debug("TSPUD-LangLoader started.");
            AssetManager.Init();

#if MELON
#else
            var harmony = new Harmony("com.sourcelocalizationteam.tspud");
            harmony.PatchAll();
#endif
        }
    }

    [HarmonyPatch]
    static class GameMasterPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMaster), "Awake")]
        public static void Awake(GameMaster __instance)
        {
            Logger.Debug("Loading font");
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
                Logger.Error($"Unable to load font", ex);
            }

            SceneManager.sceneLoaded += OnSceneLoaded;
            Logger.Debug("Loading overlay");
            InitOverlay(__instance.gameObject);
            HardcodedTextPatch.DoPatch(__instance.gameObject);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMaster), "StartMovie")]
        public static void StartMovie(bool skip, MoviePlayer player, string cameraName, ref string moviePath, bool isFullscreenMovie)
        {
            var filename = Path.GetFileName(moviePath);
#if MELON
            var new_path = Path.Combine(MelonEnvironment.GameRootDirectory, "SourceLocalization", filename);
#else
            var new_path = Path.Combine(Entrypoint.GameRootDirectory, "SourceLocalization", filename);
#endif
            if (File.Exists(new_path))
            {
                moviePath = new_path;
                Logger.Info($"Redirect video:{filename}");
            }
        }
        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.Debug($"Scene loaded: {scene.name}");

            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            Logger.Debug($"Patch Textures, count: {textures.Length}");
            using (_ = new Diagnosis("TexturePatch"))
                foreach (var texture in textures)
                {
                    try
                    {
                        var new_texture = AssetManager.Get<Texture2D>(texture.name);
                        if (new_texture == null)
                            continue;

                        if (texture.name == "cardboardbox" && scene.name != "Firewatch_UD_MASTER")
                            continue;
                        //texture.UpdateExternalTexture(new_texture.GetNativeTexturePtr());
                        Graphics.CopyTexture(new_texture, texture);
                        texture.name = $"Patched {texture.name}";
                        Logger.Debug($"Patched texture: {texture.name}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to patch texture: {texture.name} scene: {scene.name}", ex);
                    }
                }

            var meshFilters = Resources.FindObjectsOfTypeAll<MeshFilter>();
            Logger.Debug($"Patch MeshFilters, count: {meshFilters.Length}");
            using (_ = new Diagnosis("MeshPath"))
                foreach (var mr in meshFilters)
                {
                    try
                    {
                        var meshName = mr.mesh.name.Replace(" Instance", "");
                        var objName = mr.name.Replace(":", "");
                        var mesh = AssetManager.FindMesh(objName, meshName, scene.name);
                        if (mesh == null) continue;

                        Logger.Debug($"Patching origin meshName:{meshName} origin objName:{objName} {mr.name}");
                        mr.name = $"Patched {mr.name}";
                        mr.mesh = mesh;
                        mr.gameObject.AddComponent<MeshPatcher>();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to patch mesh: {mr.name},{mr.mesh.name} scene: {scene.name}", ex);
                    }
                }

            var reflectionProbes = Resources.FindObjectsOfTypeAll<ReflectionProbe>();
            Logger.Debug($"Patch ReflectionProbes, count: {reflectionProbes.Length}");
            using (_ = new Diagnosis("MeshPath"))
                foreach (var rp in reflectionProbes)
                {
                    try
                    {
                        if (rp.mode != UnityEngine.Rendering.ReflectionProbeMode.Realtime)
                            rp.mode = UnityEngine.Rendering.ReflectionProbeMode.Realtime;
                        if (rp.refreshMode != UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting)
                            rp.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
                        Logger.Debug($"Render ReflectionProbe: {rp.name}");
                        rp.RenderProbe();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to patch reflectionProbe: {rp.name} scene: {scene.name}", ex);
                    }
                }
        }

        private static void InitOverlay(GameObject gameObject)
        {
            var gameMenu = new GameObject("SourceLocalizationTeam");
            gameMenu.transform.parent = gameObject.transform;
            gameMenu.AddComponent<SourceTeamOverlay>();
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
                    if(GameMaster.Instance.languageProfileData.profiles.Any(x => x.DescriptionIni2Loc.Contains("Chinese")))
                    {
                        // Use Chinese
                    }
                    // Import translate
                    string text = AssetManager.Get<TextAsset>("translate").text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        asset.mSource.Import_CSV("", text, eSpreadsheetUpdateMode.Merge);
                        Logger.Debug($"Translate loaded");
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
                Logger.Debug("replace placeholder");
                newValue = newValue.Replace("…", newValue2);
                newValue2 = "";
            }

            originalText = originalText.Replace("The Stanley Parable ", "史丹利的寓言");
            __result = originalText.Replace("\\n", "\n").Replace("%!N!%", $"<b>{sequelCountConfigurable.GetIntValue()}</b>")
                .Replace("%!P!%", newValue).Replace(" %!S!%", newValue2);

            Logger.Debug($"final result {__result}");

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
                $"汉化版本: 2.0.0\n" +
                $"游戏版本: " + (Application.version ?? "未知");
        }
    }
}
