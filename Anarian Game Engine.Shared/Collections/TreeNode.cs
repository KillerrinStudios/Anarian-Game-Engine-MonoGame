using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anarian.Collections
{
    public class TreeNode<K, T> : IEnumerable<T> where K : IComparable
    {
        public K Key;
        public T Value;

        public TreeNode<K, T> LeftNode;
        public TreeNode<K, T> RightNode;

        public TreeNode()
        {
            //Key = null;
            //Value = 0;

            LeftNode = null;
            RightNode = null;
        }

        public TreeNode(K key, T value)
        {
            Key = key;
            Value = value;

            LeftNode = null;
            RightNode = null;
        }

        #region IEnumerator Implimentation
        public IEnumerator<T> GetEnumerator()
        {
            var queue = new Queue<TreeNode<K, T>>();

            if (this != null)
                queue.Enqueue(this);

            while (queue.Count > 0)//for (int i = 0; i < Count; i++)
            {
                var current = queue.Dequeue();
                yield return current.Value;

                if (current.LeftNode != null)
                    queue.Enqueue(current.LeftNode);
                if (current.RightNode != null)
                    queue.Enqueue(current.RightNode);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }
        #endregion
    }
}
