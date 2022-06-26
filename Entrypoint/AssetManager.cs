using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Entrypoint
{
    public class MeshInfo
    {
        public readonly string meshName;
        public readonly string sceneName;
        public readonly Mesh mesh;

        public MeshInfo(string meshName, string sceneName, Mesh mesh)
        {
            this.meshName = meshName;
            this.sceneName = sceneName;
            this.mesh = mesh;
        }
    }

    static class AssetManager
    {
        private static bool isInited = false;

        private static Stream assetStream;
        private static AssetBundle assetBundle;

        private static Dictionary<string, List<MeshInfo>> meshIndex = new Dictionary<string, List<MeshInfo>>();

        private static void BuildMeshIndex()
        {
            using (_ = new Diagnosis("AssetManager.BuildMeshIndex"))
            {
                foreach (Mesh mesh in GetAll<Mesh>())
                {
                    var meshInfo = mesh.name.Split('#');
                    string objName = meshInfo[0];
                    string meshName = "", sceneName = "";
                    if (meshInfo.Length >= 2) meshName = meshInfo[1];
                    if (meshInfo.Length >= 3) sceneName = meshInfo[2];

                    if (!meshIndex.ContainsKey(objName))
                    {
                        meshIndex[objName] = new List<MeshInfo>();
                    }

                    meshIndex[objName].Add(new MeshInfo(meshName, sceneName, mesh));
                    Logger.Debug($"Mesh {objName}.{meshName}.{sceneName} indexed.");
                }

                Logger.Debug("Final index:");
                foreach(var k in meshIndex.Keys)
                {
                    Logger.Debug($"Key: {k}, Count: {meshIndex[k].Count}");
                }
                Logger.Debug($"Build mesh index done");
            }
        }

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
                assetStream = Utils.GetResource("Resources/sourcelocalizationteam");
                assetBundle = AssetBundle.LoadFromStream(assetStream);
            }
            if (assetBundle == null)
            {
                Logger.Error("Failed to load AssetBundle!");
                assetStream.Close();
                return;
            }

            BuildMeshIndex();

            Logger.Debug($"AssetBundle Loaded");
        }

        public static T Get<T>(string name) where T : UnityEngine.Object
        {

            if (string.IsNullOrEmpty(name.Trim()))
                return null;
            return assetBundle?.LoadAsset<T>(name) ?? null;
        }

        public static T[] GetAll<T>() where T : UnityEngine.Object
        {
            return assetBundle?.LoadAllAssets<T>();
        }

        public static Mesh FindMesh(string objName, string meshName, string sceneName)
        {
            if (!meshIndex.ContainsKey(objName)) return null;
            foreach(var info in meshIndex[objName])
            {
                if ((info.meshName == "" || meshName == info.meshName) &&
                    (info.sceneName == "" || sceneName == info.sceneName)) return info.mesh;
            }

            return null;
        }
    }
}
    