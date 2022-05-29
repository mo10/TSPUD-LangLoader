using System.IO;

namespace Entrypoint
{
    public class Entrypoint
    {
        public static void Start()
        {
            File.WriteAllText("doorstop_hello.log", "Hello from Unity!");
        }
    }
}
