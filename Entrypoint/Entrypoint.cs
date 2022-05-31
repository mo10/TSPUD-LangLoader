using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Entrypoint
{
    public static class Entrypoint
    {
        private static Dictionary<string, Assembly> depends = new Dictionary<string, Assembly>();

        public static string UserDataDirectory { get; private set; }
        public static string GameManagedDirectory { get; private set; }
        public static string GameRootDirectory { get; private set; }

        public static void Start()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            GameManagedDirectory = Environment.GetEnvironmentVariable("DOORSTOP_MANAGED_FOLDER_DIR");
            GameRootDirectory = Path.GetDirectoryName(Environment.GetEnvironmentVariable("DOORSTOP_PROCESS_PATH"));
            UserDataDirectory = Path.Combine(GameRootDirectory, "UserData");

            //Logger.Debug($"GameManagedDirectory: {GameManagedDirectory}");
            //Logger.Debug($"GameRootDirectory: {GameRootDirectory}");
            //Logger.Debug($"UserDataDirectory: {UserDataDirectory}");

            LoadDependency(Assembly.GetCallingAssembly());

            var loadedAssemblies = new Dictionary<string, Assembly>();

            currentDomain.AssemblyLoad += new AssemblyLoadEventHandler(OnAssemblyLoadEventHandler);
            currentDomain.AssemblyResolve += (sender, arg) =>
            {
                if (depends.TryGetValue(arg.Name, out Assembly loadedAssembly))
                {
                    return loadedAssembly;
                }
                return null;

            };
            Logger.Debug("Waiting for Assembly-CSharp");
        }

        private static void OnAssemblyLoadEventHandler(object sender, AssemblyLoadEventArgs args)
        {
            //string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (args.LoadedAssembly.GetName().Name == "Assembly-CSharp")
            {
                using(_ = new  Diagnosis("DoPatching"))
                {
                    try
                    {

                        Patch.DoPathcing();
                    }catch(Exception ex)
                    {
                        Logger.Error($"Patch.DoPathcing", ex);
                    }
                }
            }
        }

        private static void LoadDependency(Assembly assembly)
        {
            foreach (string dependStr in assembly.GetManifestResourceNames())
            {
                string filter = $"{assembly.GetName().Name}.Resources.";
                if (dependStr.StartsWith(filter) && dependStr.EndsWith(".dll"))
                {
                    string dependName = dependStr.Remove(dependStr.LastIndexOf(".dll")).Remove(0, filter.Length);
                    if (depends.ContainsKey(dependName))
                    {
                        Logger.Error($"Dependency conflict: {dependName} First at: {depends[dependName].GetName().Name}");
                        continue;
                    }

                    Assembly dependAssembly;
                    using (var stream = assembly.GetManifestResourceStream(dependStr))
                    {
                        dependAssembly = Assembly.Load(stream.ToArray());
                    }
                    Logger.Debug($"Dependency added: {dependName}");

                    depends.Add(dependName, dependAssembly);
                }
            }
        }
    }
}
