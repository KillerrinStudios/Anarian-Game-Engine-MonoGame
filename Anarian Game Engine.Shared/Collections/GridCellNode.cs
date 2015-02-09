using Microsoft.Xna.Framework;
using System;
using System.Collections;

namespace Anarian.Collections
{
    public class GridCellNode<T>
    {
        public readonly uint ID;
        public readonly Point GridPosition;
        public T Data;

        public GridCellNode(uint id, Point gridPosition)
        {
            ID = id;
            GridPosition = gridPosition;

            Data = default(T);
        }

        public GridCellNode(uint id, Point gridPosition, T data)
        {
            ID = id;
            GridPosition = gridPosition;

            Data = data;
        }
        public override string ToString() { return "" + ID; }
    }
}