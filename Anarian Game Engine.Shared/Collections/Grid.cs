using Anarian.IDManagers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Collections
{
    public class Grid<T> : IEnumerable<T>
    {
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        List<List<T>> m_grid;
        IDManager m_gridIDManager;

        public Grid(int columns, int rows)
        {
            m_gridIDManager = new IDManager();

            Columns = columns;
            Rows = rows;

            // Create the grid using rows/columns;
            // First create the Columns
            m_grid = new List<List<T>>(Columns);

            // Now create the Rows
            for (int i = 0; i < Columns; i++) {
                m_grid.Add(new List<T>(Rows));
            }
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<T> GetEnumerator()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++ )
                {
                    yield return m_grid[x][y];
                }
            }
        }

        public T this[int column, int row]
        {
            get { return m_grid[column][row]; }
            set { m_grid[column][row] = value; }
        }
    }
}
