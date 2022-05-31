using System;
//using System.Diagnostics;

namespace Entrypoint
{
    class Diagnosis : IDisposable
    {
        //private Stopwatch stopwatch = new Stopwatch();
        private string name;

        public Diagnosis(string name)
        {
            this.name = name;
            //stopwatch.Start();
        }
        public void Dispose()
        {
            //stopwatch.Stop();
            //Logger.Debug($"[{name}] Time cost: {stopwatch.ElapsedMilliseconds} ms, {stopwatch.ElapsedTicks} ticks.");
            Logger.Debug($"[{name}]");
        }
    }
}
