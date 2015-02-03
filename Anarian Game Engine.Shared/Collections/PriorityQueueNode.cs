using System;
using System.Collections;

namespace Anarian.Collections
{
    public class PriorityQueueNode<T> : IComparable<PriorityQueueNode<T>>
    {
        public T Data;
        public readonly int Priority;

        public PriorityQueueNode(T data, int priority)
        {
            Data = data;
            Priority = priority;
        }

        public override string ToString()
        {
            return "Priority: " + Priority;
        }

        public int CompareTo(PriorityQueueNode<T> other)
        {
            if (Priority < other.Priority) return -1;
            else if (Priority > other.Priority) return 1;
            else return 0;
        }
    }
}