namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class TraitSet<T> : IEnumerable<T>
    {
        protected Cell<T> traits;

        protected TraitSet() { }

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

        protected Cell<T> Link(Cell<T> cell, Cell<T> next)
        {
            return cell == null
                ? traits    = next
                : cell.Next = next;
        }
    }
}
