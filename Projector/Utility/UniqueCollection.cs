namespace Projector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class UniqueCollection<T> : ICollection<T>
    {
        private readonly HashSet<T> set;
        private readonly List   <T> list;

        public UniqueCollection()
        {
            set  = new HashSet<T>();
            list = new List   <T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public void Add(T item)
        {
            if (set .Add(item))
                list.Add(item);
        }

        public bool Remove(T item)
        {
            return set .Remove(item)
                && list.Remove(item);
        }

        public void Clear()
        {
            set .Clear();
            list.Clear();
        }

        public bool Contains(T item)
        {
            return set.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            list.CopyTo(array, index);
        }

        public T[] ToArray()
        {
            return list.ToArray();
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
