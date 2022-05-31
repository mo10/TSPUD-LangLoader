using System;
using System.Diagnostics;
using System.IO;

namespace Entrypoint
{
    class Logger
    {
        private static StreamWriter fs = File.CreateText("sourceteam.log");
        private static object locker = new object();

        public static void Debug(object obj)
        {
#if DEBUG
            var frame = new StackTrace().GetFrame(1);
            var className = frame.GetMethod().ReflectedType.Name;
            var methodName = frame.GetMethod().Name;
            AddLog("DEBUG", className, methodName, obj);
#endif
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
    }
}
