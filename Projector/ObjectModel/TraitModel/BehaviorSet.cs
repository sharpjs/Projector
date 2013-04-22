namespace Projector.ObjectModel
{
    using System;

    internal sealed class BehaviorSet : TraitSet<IProjectionBehavior>
    {
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
                var state = trait.GetTraitOptions().AllowMultiple
                    ? MergeStates.None
                    : MergeStates.Removing;
                MergeBehavior(trait, trait.Priority, state);
            }
        }

        private void MergeBehavior(IProjectionBehavior targetBehavior, int targetPriority, MergeStates state)
        {
            // CASES:
            // 0: Merging same behavior instance: NOP. (same instance will also  have same priority)
            // 1: Merging same behavior type,     allowing multiple: INSERT
            // 2: Merging same behavior type, NOT allowing multiple: REPLACE (REMOVE & INSERT)
            // 3: Merging different behavior type: INSERT
            //
            // Assume: behaviors != null

            var current    = traits;
            var previous   = null as Cell<IProjectionBehavior>;
            var stage      = 0;
            var behavior   = traits.Item;
            var targetType = 0 != (state & MergeStates.Removing) ? targetBehavior.GetType() : null;

            for (;;)
            {
                switch (stage)
                {
                    case 0: // Find insert and remove points
                        if (behavior.Priority <= targetPriority)
                        {
                            if (behavior.Priority == targetPriority && behavior == targetBehavior)
                                // Same behavior is already present; nothing to do
                                return;

                            // Insert
                            previous = Link(previous, Cell.Cons(targetBehavior, current));
                            state |= MergeStates.Inserted;

                            if (0 == (state & MergeStates.Removed))
                                // Still looking for something to remove, or if the same behavior is already present
                                { stage++; goto case 1; }
                            else
                                // Already found remove point; our work is done here
                                return;
                        }
                        break;

                    case 1: // Find remove point, check if same behavior is already present
                        if (behavior.Priority != targetPriority)
                        {
                            if (0 != (state & MergeStates.Removing))
                                // Still looking for something to remove
                                stage++;
                            else
                                // Already have insert, not removing; we're done
                                return;
                        }
                        else if (behavior == targetBehavior)
                        {
                            // Same behavior is already present; nothing to do
                            return;
                        }
                        break;

                    case 2: // Find remove point
                        // Do nothing
                        break;
                }

                if (0 != (state & MergeStates.Removing) && behavior.GetType() == targetType)
                {
                    // Remove
                    current = Link(previous, current.Next);

                    if (stage == 0)
                        // Still need find insert point
                        state = MergeStates.Removed; //removing = false;
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

            if (0 != (state & MergeStates.Inserted))
                return;

            // Insert at end of behaviors list
            Link(previous, Cell.Cons(targetBehavior));
        }

        [Flags]
        private enum MergeStates
        {
            None     = 0x00,
            Removing = 0x01,
            Removed  = 0x02,
            Inserted = 0x04
        }
    }
}
