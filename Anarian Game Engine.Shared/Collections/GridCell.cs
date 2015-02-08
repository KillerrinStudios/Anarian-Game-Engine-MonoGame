using System;
using System.Collections;

namespace Anarian.Collections
{
    public struct GridCell<T>
    {
        public readonly int ID;
        T Data;

        public GridCell(int id)
        {
            ID = id;
            Data = default(T);
        }

        public GridCell(int id, T data)
        {
            ID = id;
            Data = data;
        }
        public override string ToString() { return "" + ID; }
    }
}