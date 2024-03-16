using System;
using System.Diagnostics;
using System.IO;

#if MELON
using MelonLoader;
#endif

namespace Entrypoint
{
    class Logger
    {
#if MELON
        public static void Debug(object obj) => MelonLogger.Msg(obj);
        public static void Error(object obj, Exception ex = null) => MelonLogger.Error($"{obj} ex:{ex}");
        public static void Info(object obj) => MelonLogger.Msg(obj);
#elif DEBUG
        private static StreamWriter fs = File.CreateText("sourceteam.log");
        private static object locker = new object();

        public static void Debug(object obj)
        {
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog("DEBUG", className, methodName, obj);
        }
        public static void Error(object obj, Exception ex = null)
        {
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog("Error", className, methodName, $"{obj} {ex?.Message}");
        }
        public static void Info(object obj)
        {
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog("Info", className, methodName, obj);
        }

        public static void AddLog(string level, string className, string methodName, object obj)
        {
            var text = $"[{level}][{className}.{methodName}]: {obj}";

            lock (locker)
            {
                fs.WriteLine(text);
                fs.Flush();
            }
        }
#else
        public static void Debug(object obj) { }
        public static void Error(object obj, Exception ex = null) { }
        public static void Info(object obj) { }
#endif
    }
}
