namespace Projector
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class CellEnumerator<T> : IEnumerator<T>
    {
        private Cell<T> cell;
        private Cell<T> next;

        public CellEnumerator(Cell<T> cell)
        {
            this.cell = null;
            this.next = cell;
        }

        public T Current
        {
            get
            {
                if (cell == null)
                    throw Error.EnumeratorNoCurrentItem();

                return cell.Item;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if ((cell = next) == null)
                return false;

            next = cell.Next;
            return true;
        }

        void IEnumerator.Reset()
        {
            throw Error.NotSupported();
        }

        public void Dispose() { }
    }
}
