namespace Projector.ObjectModel
{
    using System;

    internal sealed class BehaviorSet : TraitSet<IProjectionBehavior>
    {
        private enum RemovalState { None, Removing, Removed }

        internal override void Apply(IProjectionBehavior trait)
        {
            if (traits == null)
            {
                // Add first behavior
                traits = Cell.Cons(trait);
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

        private void MergeBehavior(IProjectionBehavior targetBehavior, int targetPriority, RemovalState removal)
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
                                return;

                            // Insert
                            previous = Link(previous, Cell.Cons(targetBehavior, current));
                            inserted = true;

                            if (removal != RemovalState.Removed)
                                // Still looking for something to remove, or if the inserted behavior was already present
                                { stage = 1; goto case 1; }
                            else
                                // Already found remove point; our work is done here
                                return;
                        }
                        break;

                    case 1: // Find remove point, check if inserted behavior was already present
                        if (behavior.Priority != targetPriority)
                        {
                            if (removal == RemovalState.Removing)
                                // Still looking for something to remove
                                stage = 2;
                            else
                                // Already have insert, not removing; we're done
                                return;
                        }
                        else if (behavior == targetBehavior)
                        {
                            // Inserted behavior was already present; remove old cell
                            Link(previous, current.Next);
                            return;
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
                        return;
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

            if (inserted)
                return;

            Link(previous, Cell.Cons(targetBehavior));
        }
    }
}
