using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Collections
{
    public class ThreadSafeList<T> : IEnumerable<T>
    {
        private object m_lockObject = new object();
        List<T> m_data;

        public ThreadSafeList()
        {
            m_data = new List<T>();
        }
        public ThreadSafeList(int capacity)
        {
            m_data = new List<T>(capacity);
        }
        public ThreadSafeList(IEnumerable<T> collection)
        {
            m_data = new List<T>(collection);
        }

        #region Collection Settings
        public int Count
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_data.Count;
                }
            }
        }
        public void Add(T c)
        {
            lock (m_lockObject)
            {
                m_data.Add(c);
            }
        }

        public T Get(int index)
        {
            lock (m_lockObject)
            {
                return m_data[index];
            }
        }

        public void Remove(int index)
        {
            lock (m_lockObject)
            {
                m_data.RemoveAt(index);
            }
        }
        public bool Remove(T c)
        {
            lock (m_lockObject)
            {
                bool result = m_data.Remove(c);
                return result;
            }
        }

        public List<T> Clone()
        {
            List<T> clonedList = new List<T>();
            lock (m_lockObject)
            {
                foreach (var i in m_data)
                {
                    clonedList.Add(i);
                }
            }
            return clonedList;
        }
        public void Clear()
        {
            lock (m_lockObject)
            {
                m_data.Clear();
            }
        }
        #endregion

        public T this[int i]
        {
            get
            {
                lock (m_lockObject)
                {
                    return m_data[i];
                }
            }
            set
            {
                lock (m_lockObject)
                {
                    m_data[i] = value;
                }
            }
        }

        #region Enumerator
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of a cloned instance of the list.
        /// Any chances made to the collection will not be reflected to the actual data
        /// </summary>
        /// <returns>The IEnumerator for the cloned list</returns>
        public IEnumerator<T> GetEnumerator()
        {
            List<T> clonedList = Clone();

            for (int i = 0; i < clonedList.Count; i++)
            {
                yield return clonedList[i];
            }
        }
        #endregion
    }
}
