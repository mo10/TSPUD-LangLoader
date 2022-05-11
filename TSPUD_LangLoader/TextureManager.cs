using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TSPUD_LangLoader
{
    public static class TextureManager
    {
        private static string basePath = Path.Combine(MelonUtils.UserDataDirectory, $"textures");

        private static List<string> Textures = new List<string>();

        public static void Init()
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            Textures.Clear();
            foreach (var filePath in Directory.GetFiles(basePath, "*.png", SearchOption.TopDirectoryOnly))
            {
                var name = Path.GetFileNameWithoutExtension(filePath);
                Textures.Add(name);
            }
#if DEBUG
            MelonLogger.Msg($"Indexed textures: {Textures.Count}");
#endif
        }

        public static bool PatchTexture(Texture2D texture)
        {
            if (Textures.Contains(texture.name))
            {
                // Load texture
                texture.hideFlags = HideFlags.HideAndDontSave;
                string filename = Path.Combine(basePath, $"{texture.name}.png");

                try
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        texture.LoadImage(stream.ToArray());
                        //LoadedTextures.Add(texture.name, texture);
                        texture.name = $"Patched {texture.name}";
#if DEBUG
                        MelonLogger.Msg($"Patched texture: {texture.name}");
#endif
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MelonLogger.Error($"Cannot patch texture: {filename}", ex);
                }
            }
            return false;
        }
    }
}
