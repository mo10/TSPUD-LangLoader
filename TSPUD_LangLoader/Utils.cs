using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPUD_LangLoader
{
    public static class Utils
    {

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
