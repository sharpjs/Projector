namespace Projector.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class InternalList<T> : IList<T>
    {
        private T[] items;
        private int count;

        private const int
            DefaultCapacity = 4,
            MaximumCapacity = int.MaxValue;

        public InternalList()
        {
            items = new T[count = DefaultCapacity];
        }

        internal T[] Items
        {
            get { return items; }
        }

        public int Count
        {
            get { return count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }

        T IList<T>.this[int index]
        {
            get { return this[index]; }
            set { throw Error.ReadOnlyCollection(); }
        }

        internal T this[int index]
        {
            get { return items[Validate(index)]; }
            set { items[Validate(index)] = value; }
        }

        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < items.Length; i++)
                if (comparer.Equals(items[i], item))
                    return true;

            return false;
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < items.Length; i++)
                if (comparer.Equals(items[i], item))
                    return i;

            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (items as IList<T>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        void ICollection<T> .Add      (T item)            { throw Error.ReadOnlyCollection(); }
        bool ICollection<T> .Remove   (T item)            { throw Error.ReadOnlyCollection(); }
        void ICollection<T> .Clear    ()                  { throw Error.ReadOnlyCollection(); }
        void IList<T>       .Insert   (int index, T item) { throw Error.ReadOnlyCollection(); }
        void IList<T>       .RemoveAt (int index)         { throw Error.ReadOnlyCollection(); }

        internal void Add(T item)
        {
            var items = this.items;
            var count = this.count;
            if (count == items.Length)
                items = Grow();
            items[count] = item;
            this.count = count + 1;
        }

        internal void Insert(int index, T item)
        {
            Validate(index);
            var items = this.items;
            var count = this.count;
            if (count == items.Length)
                items = Grow();
            if (index != count)
                Array.Copy(items, index, items, index + 1, count - index);
            items[count] = item;
            this.count = count + 1;
        }

        internal bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }

        internal void RemoveAt(int index)
        {
            var items = this.items;
            var count = this.count - 1;
            if (index > count)
                throw Error.ArgumentOutOfRange("index");
            if (index != count)
                Array.Copy(items, index + 1, items, index, count - index);
            items[count] = default(T);
            this.count = count;
        }

        internal void Clear()
        {
            var count = this.count;
            if (count == 0) return;
            Array.Clear(items, 0, count);
            this.count = 0;
        }

        private T[] Grow()
        {
            var oldArray = items;
            var capacity = oldArray.Length;
            if (capacity == int.MaxValue)
                throw Error.InternalError("Cannot grow InternalList beyond max int value.");

            capacity = capacity > MaximumCapacity / 2
                ? MaximumCapacity
                : capacity * 2;

            var newArray = new T[capacity];
            Array.Copy(oldArray, 0, newArray, 0, count);
            return items = newArray;
        }

        private int Validate(int index)
        {
            if (index < 0 || index >= count)
                throw Error.ArgumentOutOfRange("index");

            return index;
        }
    }
}
