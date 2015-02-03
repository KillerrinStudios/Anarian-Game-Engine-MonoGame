using System;
using System.Collections.Generic;

namespace Anarian.Collections
{
    public class PriorityQueue<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private List<T> m_data;

        public PriorityQueue()
        {
            m_data = new List<T>();
        }

        public void Enqueue(T item)
        {
            m_data.Add(item);
            int ci = m_data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (m_data[ci].CompareTo(m_data[pi]) >= 0) break;   // if child item is larger than (or equal) parent so we're done
                T tmp = m_data[ci]; m_data[ci] = m_data[pi]; m_data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling node
            int li = m_data.Count - 1;  // last index (before removal)
            T frontItem = m_data[0]; // fetch the front
            m_data[0] = m_data[li];
            m_data.RemoveAt(li);

            --li;   // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1;    // left child index of parent
                if (ci > li) { break; }     // no children, so done

                int rc = ci + 1;        // right child
                if (rc <= li &&                             // if there is a rc (ci + 1), and it is smaller
                    m_data[rc].CompareTo(m_data[ci]) < 0)
                {  // than the left child, use the rc instead
                    ci = rc;
                }

                if (m_data[pi].CompareTo(m_data[ci]) <= 0)
                { // parent is smaller than (or equal to) smallest child so done
                    break;
                }

                T tmp = m_data[pi]; m_data[pi] = m_data[ci]; m_data[ci] = tmp;  // swap parent and child
                pi = ci;
            }

            return frontItem;
        }

        public T Peek()
        {
            T frontItem = m_data[0];
            return frontItem;
        }

        public int Count
        {
            get { return m_data.Count; }
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < m_data.Count; i++)
            {
                s += m_data[i].ToString() + " ";
            }
            s += "Count = " + m_data.Count;
            return s;
        }

        public bool IsConsistent()
        {
            if (m_data.Count == 0) return true;
            int li = m_data.Count - 1;  // last index

            for (int pi = 0; pi < m_data.Count; ++pi)
            { // each parent index
                int lci = 2 * pi + 1;   // left child index
                int rci = 2 * pi + 2;   // right child index

                if (lci <= li && m_data[pi].CompareTo(m_data[lci]) > 0) return false;   // if lc exists, and its greater than parent than bad
                if (rci <= li && m_data[pi].CompareTo(m_data[rci]) > 0) return false;   // check the right child too
            }
            return true;    // passed all checks
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T o in m_data)
            {
                yield return o;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public T this[int key]
        {
            get
            {
                return m_data[key];
            }
        }
    }
}