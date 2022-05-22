using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TSPUD_LangLoader
{
    public static class Utils
    {
        /// <summary>
        /// Read embedded Resource from this assembly.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Stream GetResource(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"{Assembly.GetExecutingAssembly().GetName().Name}.{file.Replace('/','.')}");
            return stream;
        }
        /// <summary>
        /// Read all bytes from a Stream.
        /// </summary>
        /// <param name="steamReader"></param>
        /// <returns></returns>
        public static byte[] ToArray(this Stream steamReader)
        {
            var buffer = new byte[steamReader.Length];
            steamReader.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
