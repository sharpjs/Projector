namespace Projector
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal static class Cell
    {
        public static Cell<T> Cons<T>(T item)
        {
            return new Cell<T>(item);
        }

        public static Cell<T> Cons<T>(T item, Cell<T> next)
        {
            return new Cell<T>(item, next);
        }

        public static void Append<T>(ref Cell<T> location, Cell<T> cell)
        {
            var c = location;
            if (c == null)
                location = cell;
            else
                c.Next = cell;
        }
    }

    internal sealed class Cell<T> : IEnumerable<T>
    {
        private readonly T item;
        private Cell<T>    next;

        public Cell(T item)
        {
            this.item = item;
        }

        public Cell(T item, Cell<T> next)
        {
            this.item = item;
            this.next = next;
        }

        public T Item
        {
            get { return item; }
        }

        public Cell<T> Next
        {
            get { return next; }
            set { next = value; }
        }

        public Cell<T> Copy()
        {
            return new Cell<T>(item, next);
        }

        public override string ToString()
        {
            var text = new StringBuilder(128).Append('[');
            var cell = this;
            T   item;

            for (;;)
            {
                text.Append
                   (
                    (null != (item = cell.item))
                        ? item.ToString()
                        : "(null)"
                );

                if (null != (cell = cell.next))
                    text.Append(" -> ");
                else
                    break;
            }

            return text.Append(']').ToString();
        }

        public CellEnumerator<T> GetEnumerator()
        {
            return new CellEnumerator<T>(this);
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
