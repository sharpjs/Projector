namespace Projector.ObjectModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(BehaviorCollection.DebugView))]
    public sealed class BehaviorCollection : ICollection<IProjectionBehavior>
    {
        private Cell<IProjectionBehavior> traits;
        private int                       count;

        internal BehaviorCollection() { }

        public int Count
        {
            get { return count; }
        }

        internal Cell<IProjectionBehavior> First
        {
            get { return traits; }
        }

        public bool Contains(IProjectionBehavior behavior)
        {
            for (var cell = traits; cell != null; cell = cell.Next)
                if (cell.Item == behavior)
                    return true;

            return false;
        }

        public void CopyTo(IProjectionBehavior[] array, int index)
        {
            var i = index;
            for (var cell = traits; cell != null; cell = cell.Next)
                array[i++] = cell.Item;
        }

        public IEnumerator<IProjectionBehavior> GetEnumerator()
        {
            return (traits ?? Enumerable.Empty<IProjectionBehavior>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void AddInternal(IProjectionBehavior trait)
        {
            if (traits == null)
            {
                // Add first behavior
                traits = Cell.Cons(trait);
                count  = 1;
            }
            else
            {
                // Merge subsequent behavior
                var removal = trait.GetTraitOptions().AllowMultiple
                    ? RemovalState.None
                    : RemovalState.Removing;
                MergeBehavior(trait, trait.Priority, removal);
            }
        }

        private enum RemovalState { None, Removing, Removed }

        private bool MergeBehavior(IProjectionBehavior targetBehavior, int targetPriority, RemovalState removal)
        {
            // CASES:
            // 0: Merging same behavior instance: MOVE to head of its priority group (REMOVE & INSERT)
            // 1: Merging same behavior type,     allowing multiple: INSERT
            // 2: Merging same behavior type, NOT allowing multiple: REPLACE (REMOVE & INSERT)
            // 3: Merging different behavior type: INSERT
            //
            // Assume: behaviors != null
            // Note: same instance will also have same priority

            var current    = traits;
            var previous   = null as Cell<IProjectionBehavior>;
            var stage      = 0;
            var behavior   = traits.Item;
            var targetType = (removal == RemovalState.Removing) ? targetBehavior.GetType() : null;
            var inserted   = false;

            for (;;)
            {
                switch (stage)
                {
                    case 0: // Find insert and remove points
                        if (behavior.Priority <= targetPriority)
                        {
                            if (behavior == targetBehavior)
                                // Same behavior is already first for its priority; nothing to do
                                return false;

                            // Insert
                            previous = Link(previous, Cell.Cons(targetBehavior, current));
                            inserted = true;

                            if (removal != RemovalState.Removed)
                                // Still looking for something to remove, or if the inserted behavior was already present
                                { stage = 1; goto case 1; }
                            else
                                // Already found remove point; our work is done here
                                return false;
                        }
                        break;

                    case 1: // Find remove point, check if inserted behavior was already present
                        if (behavior.Priority < targetPriority)
                        {
                            if (removal == RemovalState.Removing)
                                // Still looking for something to remove
                                stage = 2;
                            else
                                // Already have insert, not removing; we're done
                                return true;
                        }
                        else if (behavior == targetBehavior)
                        {
                            // Inserted behavior was already present; remove old cell
                            Link(previous, current.Next);
                            return false;
                        }
                        break;

                    case 2: // Find remove point
                        // Do nothing
                        break;
                }

                if (removal == RemovalState.Removing && behavior.GetType() == targetType)
                {
                    // Remove
                    current = Link(previous, current.Next);

                    if (stage == 0)
                        // Still need find insert point
                        removal = RemovalState.Removed;
                    else
                        // Already have insert and remove points; we're done
                        return false;
                }
                else
                {
                    // Advance to next behavior
                    previous = current;
                    current  = current.Next;
                }

                if (current == null)
                    break;

                behavior = current.Item;
            }

            // Either:
            // (A) All existing items have higher priority; insert at end.
            // (B) Found insert point, but never found remove point.

            if (!inserted)
                // Insert at end
                Link(previous, Cell.Cons(targetBehavior));

            return true;
        }

        private Cell<IProjectionBehavior> Link(Cell<IProjectionBehavior> cell, Cell<IProjectionBehavior> next)
        {
            return cell == null
                ? traits    = next
                : cell.Next = next;
        }

        bool ICollection<IProjectionBehavior>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<IProjectionBehavior>.Add(IProjectionBehavior item)
        {
            throw Error.ReadOnlyCollection();
        }

        bool ICollection<IProjectionBehavior>.Remove(IProjectionBehavior item)
        {
            throw Error.ReadOnlyCollection();
        }

        void ICollection<IProjectionBehavior>.Clear()
        {
            throw Error.ReadOnlyCollection();
        }

        internal sealed class DebugView
        {
            private readonly BehaviorCollection collection;

            public DebugView(BehaviorCollection collection)
            {
                if (collection == null)
                    throw new ArgumentNullException("collection");

                this.collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IProjectionBehavior[] Items
            {
                get
                {
                    var array = new IProjectionBehavior[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
            }
        }
    }
}
