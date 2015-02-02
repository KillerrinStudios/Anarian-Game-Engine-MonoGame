using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.IDManagers
{
    public static class AnarianObjectIDManager
    {
        public static uint CurrentID { get; set; }

        public static uint GetNewID()
        {
            uint IDToUse = CurrentID;
            IncrimentID();

            return IDToUse;
        }

        private static void IncrimentID()
        {
            if (CurrentID < uint.MaxValue)
                CurrentID++;
            else
                CurrentID = uint.MinValue;
        }
    }
}
