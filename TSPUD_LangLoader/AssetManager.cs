using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace TSPUD_LangLoader
{
    static class AssetManager
    {
        private static bool isInited = false;

#if BUNDLE
        private static Stream assetStream;
        private static AssetBundle assetBundle;
        private static Dictionary<string, UnityEngine.Object> allAssets = new Dictionary<string, UnityEngine.Object>();
#else
        private static string baseFontPath = Path.Combine(MelonUtils.UserDataDirectory, $"fonts");
        private static string baseTexturePath = Path.Combine(MelonUtils.UserDataDirectory, $"textures");
        private static string baseTextFile = Path.Combine(MelonUtils.UserDataDirectory, $"translate.csv");

        private static Dictionary<string, string> allAssets = new Dictionary<string, string>();
        private static Dictionary<string, UnityEngine.Object> assetCaches = new Dictionary<string, UnityEngine.Object>();
#endif
        public static void Init()
        {
            if (isInited)
                return;
#if BUNDLE
            if (assetBundle != null) assetBundle.Unload(true);

            if (assetStream != null) assetStream.Close();

            using (_ = new Diagnosis("Utils.GetResource"))
                assetStream = Utils.GetResource("resources/sourceteam");
            using (_ = new Diagnosis("AssetBundle.LoadFromStream"))
                assetBundle = AssetBundle.LoadFromStream(assetStream);

            if (assetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                assetStream.Close();
                return;
            }

            using (_ = new Diagnosis("AssetBundle.LoadAllAssets"))
                foreach (var asset in assetBundle.LoadAllAssets())
                {
                    allAssets.Add(asset.name, asset);
#if DEBUG
                    MelonLogger.Msg($"Loaded asset: {asset.name}");
#endif
                }
#else
            var mapping1 = GetFileMapping(baseFontPath,"*.ttf");
            var mapping2 = GetFileMapping(baseTexturePath, "*.png");
            allAssets = allAssets.Concat(mapping1).Concat(mapping2).ToDictionary(x => x.Key, x => x.Value);

            if (File.Exists(baseTextFile))
            {
                allAssets.Add(Path.GetFileNameWithoutExtension(baseTextFile), baseTextFile);
            }
#endif
            isInited = true;
            MelonLogger.Msg($"Loaded {allAssets.Count} asset(s)");
        }

        public static T Get<T>(string name) where T : UnityEngine.Object
        {
            if (!allAssets.ContainsKey(name))
                return default(T);
#if BUNDLE
            return (T)allAssets[name];
#else
            UnityEngine.Object obj = null;
            if (assetCaches.ContainsKey(name))
            {
                obj = assetCaches[name];
                return (T)obj;
            }

            try
            {
                if (typeof(T) == typeof(Texture2D))
                {
                    using (var stream = File.OpenRead(allAssets[name]))
                    {
                        var texture = new Texture2D(1, 1);
                        texture.hideFlags = HideFlags.HideAndDontSave;
                        texture.LoadImage(stream.ToArray());
                        assetCaches.Add(name, texture);

                        obj = texture;
                    }
                }
                else if (typeof(T) == typeof(Font))
                {
                    Font font = new Font(allAssets[name]);
                    assetCaches.Add(name, font);

                    obj = font;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Cannot load texture: {name}", ex);
            }
#if DEBUG
            if(obj) MelonLogger.Msg($"Loaded: {filePath}");
#endif
            return (T)obj;
#endif
        }

        public static string GetText(string name)
        {
            if (!allAssets.ContainsKey(name))
                return null;
#if BUNDLE
            return ((TextAsset)allAssets[name]).text;
#else
            return File.ReadAllText(allAssets[name]);
#endif
        }

        private static Dictionary<string, string> GetFileMapping(string path, string searchPattern)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Dictionary<string, string> mapping = new Dictionary<string, string>();
            foreach (var filePath in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(filePath);
                mapping.Add(name, filePath);
#if DEBUG
                MelonLogger.Msg($"Indexed asset: {name}");
#endif
            }
            return mapping;
        }
    }
}
