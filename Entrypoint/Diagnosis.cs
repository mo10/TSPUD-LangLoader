using System;
#if DEBUG
using System.Diagnostics;
#endif

namespace Entrypoint
{
    class Diagnosis : IDisposable
    {
#if DEBUG
        private Stopwatch stopwatch = new Stopwatch();
        private string name;

        public Diagnosis(string name)
        {
            this.name = name;
            stopwatch.Start();
        }
        public void Dispose()
        {
            stopwatch.Stop();
            Logger.Debug($"[{name}] Time cost: {stopwatch.ElapsedMilliseconds} ms, {stopwatch.ElapsedTicks} ticks.");
            Logger.Debug($"[{name}]");
        }
#else
        public Diagnosis(string name) { }
        public void Dispose() { }
#endif
    }
}
