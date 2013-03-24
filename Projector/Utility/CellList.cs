namespace Projector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    internal class CellList<T> : ICollection<T>
    {
        private Cell<T> head;
        private Cell<T> tail;
        private int     count;

        public Cell<T> Head
        {
            get { return head; }
        }

        public int Count    
        {
            get { return count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public Cell<T> Push(T item) // add at head
        {
            head = (head != null)
                ?        Cell.Cons(item, head)
                : tail = Cell.Cons(item);
            count++;
            return head;
        }

        public Cell<T> Enqueue(T item) // add at tail
        {
            tail = (head != null)
                ? tail.Next = Cell.Cons(item)
                : head      = Cell.Cons(item);
            count++;
            return tail;
        }

        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }

        public bool Remove(T item)
        {
            var cell     = null as Cell<T>;
            var next     = head;
            var comparer = EqualityComparer<T>.Default;

            while (next != null)
            {
                if (comparer.Equals(next.Item, item))
                {
                    if (cell == null)
                        head = next.Next;
                    else
                        cell.Next = next.Next;
                    count--;
                    return true;
                }

                cell = next;
                next = next.Next;
            }

            return false;
        }

        public bool TryTake(out T item) // remove at head
        {
            if (head != null)
            {
                item = head.Item;
                head = head.Next;
                if (head == null) tail = null;
                count--;
                return true;
            }
            else
            {
                item = default(T);
                return false;
            }
        }

        public void Clear()
        {
            head = tail = null;
            count = 0;
        }

        public bool Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;

            for (var cell = head; cell != null; cell = cell.Next)
                if (comparer.Equals(cell.Item, item))
                    return true;

            return false;
        }

        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw Error.ArgumentNull("array");
            if (index < 0 || index > array.Length - count)
                throw Error.ArgumentOutOfRange("index");

            for (var cell = head; cell != null; cell = cell.Next)
                array[index++] = cell.Item;
        }

        public CellEnumerator<T> GetEnumerator()
        {
            return new CellEnumerator<T>(head);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
