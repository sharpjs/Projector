namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(TraitCollection.DebugView))]
    public sealed class TraitCollection : ICollection<object>
    {
        private Entry[] entries;
        private int     count;

        internal TraitCollection()
        {
            entries = Empty;
        }

        public int Count
        {
            get { return count; }
        }

        bool ICollection<object>.IsReadOnly
        {
            get { return true; }
        }

        public bool Contains(object trait)
        {
            var entries = this.entries;

            for (var i = 0; i < entries.Length; i++)
                if (entries[i].Trait == trait)
                    return true;

            return false;
        }

        internal int FindInheritable(int start, out object trait)
        {
            var entries = this.entries;

            for (var i = start; i < entries.Length; i++)
            {
                var entry = entries[i];
                if (entry.Inheritable)
                {
                    trait = entry.Trait;
                    return i;
                }
            }

            trait = null;
            return NotFound;
        }

        public void CopyTo(object[] array, int index)
        {
            if (array == null)
                throw Error.ArgumentNull("array");
            if (array.Rank != 1 || array.GetLowerBound(0) != 0)
                throw Error.ArgumentOutOfRange("array");

            var entries = this.entries;
            if (index < 0 || array.Length - index < entries.Length)
                throw Error.ArgumentOutOfRange("index");

            for (var i = index; i < entries.Length; i++)
                array[i] = entries[i].Trait;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(entries);
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void AddInternal(object trait, bool inheritable)
        {
            var entry = new Entry(trait, inheritable);

            if (count == 0)
            {
                // Add first trait
                Append(entry);
            }
            else if (trait.GetTraitOptions().AllowMultiple)
            {
                // Add subsequent non-singleton traits
                AddAllowMultiple(entry);
            }
            else
            {
                // Merge subsequent singleton traits
                AddAllowSingle(entry);
            }
        }

        private void AddAllowMultiple(Entry entry)
        {
            var entries = this.entries;
            var trait   = entry.Trait;

            // Detect if same trait already present
            for (var i = count - 1; i >= 0; i--)
            {
                if (entries[i].Trait == trait)
                    // Same trait already present; nothing to do
                    return;
            }

            // Same trait not present; add
            Append(entry);
        }

        private void AddAllowSingle(Entry entry)
        {
            var entries    = this.entries;
            var targetType = entry.Trait.GetType();

            // Detect if trait of same type already present
            for (var i = count - 1; i >= 0; i--)
            {
                var candidate = entries[i].Trait;
                if (candidate.GetType() == targetType)
                {
                    if (candidate != entry.Trait)
                        // Trait of same type already present; remove it, then add
                        RemoveAndAppend(i, entry);

                    // Same trait already present; nothing to do
                    return;
                }
            }

            // No trait of same type present; add
            Append(entry);
        }

        private void Append(Entry entry)
        {
            var entries   = this.entries;
            var lastIndex = this.count;

            if (lastIndex == entries.Length)
                entries = Resize(lastIndex + 4);

            entries[lastIndex] = entry;
            count = lastIndex + 1;
        }

        private void RemoveAndAppend(int index, Entry entry)
        {
            var entries   = this.entries;
            var lastIndex = this.count - 1;

            if (index < lastIndex)
                Array.Copy(entries, index + 1, entries, index, lastIndex - index);

            entries[lastIndex] = entry;
        }

        private Entry[] Resize(int capacity)
        {
            var array = new Entry[capacity];
            Array.Copy(entries, array, count);
            return entries = array;
        }

        internal void Trim()
        {
            if (count != entries.Length)
                Resize(count);
        }

        void ICollection<object>.Add(object item)
        {
            throw Error.ReadOnlyCollection();
        }

        bool ICollection<object>.Remove(object item)
        {
            throw Error.ReadOnlyCollection();
        }

        void ICollection<object>.Clear()
        {
            throw Error.ReadOnlyCollection();
        }

        private static readonly Entry[] Empty = { };

        private const int NotFound = -1;

        internal struct Entry
        {
            public readonly object Trait;
            public readonly bool   Inheritable;

            public Entry(object trait, bool inheritable)
            {
                Trait       = trait;
                Inheritable = inheritable;
            }
        }

        public struct Enumerator : IEnumerator<object>
        {
            private readonly Entry[] entries;
            private int              index;
            private object           current;

            internal Enumerator(Entry[] entries)
            {
                this.entries = entries;
                this.index   = 0;
                this.current = null;
            }

            public object Current
            {
                get { return current; }
            }

            public bool MoveNext()
            {
                var i = index;
                if (i < entries.Length)
                {
                    index   = i + 1;
                    current = entries[i].Trait;
                    return true;
                }
                else
                {
                    current = null;
                    return false;
                }
            }

            public void Reset()
            {
                index   = 0;
                current = null;
            }

            public void Dispose() { }
        }

        internal sealed class DebugView
        {
            private readonly TraitCollection collection;

            public DebugView(TraitCollection collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                this.collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object[] Items
            {
                get
                {
                    var array = new object[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
            }
        }
    }
}
