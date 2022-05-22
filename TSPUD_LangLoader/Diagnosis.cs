using MelonLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSPUD_LangLoader
{
    class Diagnosis : IDisposable
    {
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
            MelonLogger.Msg($"[{name}] Time cost: {stopwatch.ElapsedMilliseconds} ms, {stopwatch.ElapsedTicks} ticks.");
        }
    }
}
