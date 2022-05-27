using Dummiesman;
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
#else
        private static string baseFontPath = Path.Combine(MelonUtils.UserDataDirectory, $"fonts");
        private static string baseTexturePath = Path.Combine(MelonUtils.UserDataDirectory, $"textures");
        private static string baseMeshPath = Path.Combine(MelonUtils.UserDataDirectory, $"meshes");

        private static Dictionary<string, string> fontAssets = new Dictionary<string, string>();
        private static Dictionary<string, string> textureAssets = new Dictionary<string, string>();
        private static Dictionary<string, string> meshAssets = new Dictionary<string, string>();

        private static Dictionary<string, Font> fontCaches = new Dictionary<string, Font>();
        private static Dictionary<string, Texture2D> textureCaches = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Mesh> meshCaches = new Dictionary<string, Mesh>();
#endif
        public static void Init()
        {
            if (isInited)
                return;
            isInited = true;
#if BUNDLE
            if (assetBundle != null)
                assetBundle.Unload(true);
            if (assetStream != null)
                assetStream.Close();

            using (_ = new Diagnosis("AssetBundle.LoadFromStream"))
            {
                assetStream = Utils.GetResource("resources/sourceteam");
                assetBundle = AssetBundle.LoadFromStream(assetStream);
            }
            if (assetBundle == null)
            {
                MelonLogger.Error("Failed to load AssetBundle!");
                assetStream.Close();
                return;
            }

            MelonLogger.Msg($"AssetBundle Loaded");
#else
            fontAssets = GetFileMapping(baseFontPath,"*.ttf");
            textureAssets = GetFileMapping(baseTexturePath, "*.png");
            meshAssets = GetFileMapping(baseMeshPath, "*.obj");

            MelonLogger.Msg($"Loaded {fontAssets.Count} fonts {textureAssets.Count} textures {meshAssets} meshes");
#endif
        }

        public static T Get<T>(string name) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(name.Trim()))
                return null;
#if BUNDLE
            return assetBundle?.LoadAsset<T>(name) ?? null;
#else
            UnityEngine.Object obj = null;

            if (typeof(T) == typeof(Texture2D) && textureCaches.TryGetValue(name,out var texCache))
            {
                obj = texCache;
            }
            else if (typeof(T) == typeof(Font) && fontCaches.TryGetValue(name, out var fontCache))
            {
                obj = fontCache;
            }
            else if (typeof(T) == typeof(Mesh) && meshCaches.TryGetValue(name, out var meshCache))
            {
                obj = meshCache;
            }
            if (obj)
            {
#if DEBUG
                MelonLogger.Msg($"Hit cache: {name}");
#endif
                return (T)obj;
            }

            try
            {
                if (typeof(T) == typeof(Texture2D) && textureAssets.TryGetValue(name, out var texPath))
                {
                    using (var stream = File.OpenRead(texPath))
                    {
                        var texture = new Texture2D(1, 1);
                        texture.hideFlags = HideFlags.HideAndDontSave;
                        texture.LoadImage(stream.ToArray());
                        textureCaches.Add(name, texture);

                        obj = texture;
                    }
                }
                else if (typeof(T) == typeof(Font) && fontAssets.TryGetValue(name, out var fontPath))
                {
                    Font font = new Font(fontPath);
                    fontCaches.Add(name, font);

                    obj = font;
                }
                else if (typeof(T) == typeof(Mesh) && meshAssets.TryGetValue(name, out var meshPath))
                {
                    using (var stream = File.OpenRead(meshPath))
                    {
                        var loadedObj = new OBJLoader().Load(stream);
                        var meshFilter = loadedObj.GetComponentInChildren<MeshFilter>(true);
                        if (meshFilter && meshFilter.mesh)
                        {
                            meshCaches.Add(name, meshFilter.mesh);
                            obj = meshFilter.mesh;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Cannot load: {name}", ex);
            }
#if DEBUG
            if (obj) MelonLogger.Msg($"Loaded: {name}");
#endif
            return (T)obj;
#endif
        }

        public static string GetText(string name)
        {
#if BUNDLE
            return assetBundle.LoadAsset<TextAsset>(name).text;
#else
            return File.ReadAllText(Path.Combine(MelonUtils.UserDataDirectory, $"{name}.csv"));
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
