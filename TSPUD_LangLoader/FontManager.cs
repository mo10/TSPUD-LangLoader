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
    public static class FontManager
    {
        private static string basePath = Path.Combine(MelonUtils.UserDataDirectory, $"fonts");

        public static Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();

        public static void Init()
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            foreach (var filePath in Directory.GetFiles(basePath, "*.ttf", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Font font = new Font(filePath);
                    TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(font);
                    if (fontAsset)
                    {
                        fontAsset.name = Path.GetFileNameWithoutExtension(filePath);
                        Fonts.Add(fontAsset.name, fontAsset);
#if DEBUG
                        MelonLogger.Msg($"Loaded: {filePath}");
#endif
                    }
                }
                catch(Exception ex)
                {
                    MelonLogger.Error($"Unable to load font: {filePath}",ex);
                }
            }
        }
    }
}
