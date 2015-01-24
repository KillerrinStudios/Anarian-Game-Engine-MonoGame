using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Helpers
{
    public static class DebugTools
    {
        public static void PrintTotaloMemoryInUse()
        {
            Debug.WriteLine("GC: TOTAL MEMORY {0}", GC.GetTotalMemory(false));
        }   
    }
}
