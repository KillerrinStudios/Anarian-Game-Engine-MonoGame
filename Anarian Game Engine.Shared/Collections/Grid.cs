using Anarian.IDManagers;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anarian.Collections
{
    public class Grid<T> : IEnumerable<GridCellNode<T>>
    {
        public uint TotalIDs { get { return m_gridIDManager.CurrentID; } }
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        List<List<GridCellNode<T>>> m_grid;
        IDManager m_gridIDManager;

        public Grid(int columns, int rows)
        {
            m_gridIDManager = new IDManager();

            Columns = columns;
            Rows = rows;

            // Create the grid using columns/rows;
            m_grid = new List<List<GridCellNode<T>>>(Columns);

            // Then create the Columns
            for (int i = 0; i < Columns; i++) {
                m_grid.Add(new List<GridCellNode<T>>(Rows));
            }

            // Finally, populate them with an ID'd GridCell
            int x = 0;
            foreach (var i in m_grid)
            {
                for (int y = 0; y < Rows; y++)
                {
                    i.Add(new GridCellNode<T>(m_gridIDManager.GetNewID(), new Point(x, y)));
                }
                x++;
            }
        }

        public bool InGrid(Point point)
        {
            return InGrid(point.X, point.Y);
        }
        public bool InGrid(int column, int row)
        {
            return (column >= 0) &&
                   (column < Columns) &&
                   (row >= 0) &&
                   (row <= Rows);
        }



        #region Enumerations
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<GridCellNode<T>> GetEnumerator()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++ )
                {
                    yield return m_grid[x][y];
                }
            }
        }

        public GridCellNode<T> GetGridCellNode(int column, int row) { return m_grid[column][row]; }
        public uint GetIDFromColumnRow(Point gridPos) { return GetIDFromColumnRow(gridPos.X, gridPos.Y); }
        public uint GetIDFromColumnRow(int column, int row) { return m_grid[column][row].ID; }

        public Point GetColumnRowFromID(uint id)
        {
            int x, y; 
            GetColumnRowFromID(id, out x, out y);
            return new Point(x, y);
        }
        public void GetColumnRowFromID(uint id, out int X, out int Y)
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (m_grid[x][y].ID == id)
                    {
                        X = x;
                        Y = y;
                        return;
                    }
                }
            }

            throw new IndexOutOfRangeException();
        }

        public T this [uint gridID]
        {
            get {
                Point p = GetColumnRowFromID(gridID);
                return this[p.X, p.Y];
            }
            set
            {
                Point p = GetColumnRowFromID(gridID);
                this[p.X, p.Y] = value;
            }
        }

        public T this[Point gridPos] { get { return this[gridPos.X, gridPos.Y]; } set { this[gridPos.X, gridPos.Y] = value; } }
        public T this[int column, int row]
        {
            get { return m_grid[column][row].Data; }
            set {
                var data = m_grid[column][row];
                data.Data = value;
            }
        }
        #endregion
    }
}
