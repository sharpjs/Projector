namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal abstract class TraitSet<T> : IEnumerable<T>
    {
        protected Cell<T> traits;
        protected int     ownedCount;

        protected TraitSet() { }

        protected TraitSet(TraitSet<T> parent)
        {
            traits = parent.traits;
        }

        internal Cell<T> First
        {
            get { return traits; }
        }

        public override string ToString()
        {
            return (traits != null) ? traits.ToString() : "[]";
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (traits ?? Enumerable.Empty<T>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal abstract void Apply(T trait);

        protected Cell<T> CopyShared(Cell<T> shared, Cell<T> limit)
        {
            Cell<T> source, target;

            source = (shared == null) ? traits : shared.Next;

            for (;;)
            {
                target = Cell.Cons(source.Item);
                Link(shared, target);
                ownedCount++;

                source = source.Next;
                if (source == limit)
                    break;

                shared = target;
            }

            target.Next = limit;
            return target;
        }

        protected Cell<T> Link(Cell<T> cell, Cell<T> next)
        {
            if (cell == null)
                return traits = next;
            else
                return cell.Next = next;
        }
    }
}
