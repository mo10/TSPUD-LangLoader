using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entrypoint
{
    static class AssetManager
    {
        private static bool isInited = false;

        private static Stream assetStream;
        private static AssetBundle assetBundle;

        public static void Init()
        {
            if (isInited)
                return;
            isInited = true;
            if (assetBundle != null)
                assetBundle.Unload(true);
            if (assetStream != null)
                assetStream.Close();

            using (_ = new Diagnosis("AssetBundle.LoadFromStream"))
            {
                assetStream = Utils.GetResource("Resources/sourceteam");
                assetBundle = AssetBundle.LoadFromStream(assetStream);
            }
            if (assetBundle == null)
            {
                Logger.Error("Failed to load AssetBundle!");
                assetStream.Close();
                return;
            }

            Logger.Debug($"AssetBundle Loaded");
        }

        public static T Get<T>(string name) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(name.Trim()))
                return null;
            return assetBundle?.LoadAsset<T>(name) ?? null;
        }
    }
}
