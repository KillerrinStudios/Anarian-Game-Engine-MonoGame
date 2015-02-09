using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.IDManagers
{
    public class IDManager
    {
        private object lockObject;

        public uint CurrentID { get; set; }

        public IDManager()
        {
            lockObject = new object();
            CurrentID = 0;
        }

        public void Reset() { CurrentID = 0; }

        public uint GetNewID()
        {
            uint IDToUse;
            lock (lockObject)
            {
                IDToUse = CurrentID;
                IncrimentID();
            }

            return IDToUse;
        }

        private void IncrimentID()
        {
            if (CurrentID < uint.MaxValue)
                CurrentID++;
            else
                CurrentID = uint.MinValue;
        }
    }
}
