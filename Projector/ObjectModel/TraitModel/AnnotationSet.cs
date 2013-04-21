namespace Projector.ObjectModel
{
    using System;

    internal sealed class AnnotationSet : TraitSet<object>
    {
        public AnnotationSet()
            : base() { }

        public AnnotationSet(AnnotationSet parent)
            : base(parent) { }

        internal override void Apply(object trait)
        {
            if (traits == null)
            {
                // Add first annotation
                traits = Cell.Cons(trait);
                ownedCount++;
            }
            else if (trait.GetTraitOptions().AllowMultiple)
            {
                // Add subsequent non-singleton annotations
                MergeAnnotationAllowMultiple(trait);
            }
            else
            {
                // Merge subsequent singleton annotations
                MergeAnnotationAllowSingle(trait);
            }
        }

        private void MergeAnnotationAllowMultiple(object targetAnnotation)
        {
            for (var trait = traits; trait != null; trait = trait.Next)
                if (trait.Item == targetAnnotation)
                    // Same annotation already present; nothing to do
                    return;

            // Insert
            traits = Cell.Cons(targetAnnotation, traits);
            ownedCount++;
        }

        private void MergeAnnotationAllowSingle(object targetAnnotation)
        {
            var current    = traits;
            var previous   = null as Cell<object>;
            var index      = 0;
            var targetType = targetAnnotation.GetType();
            var shared     = null as Cell<object>;

            while (current != null)
            {
                if (index == ownedCount)
                    shared = previous;

                if (current.Item.GetType() != targetType)
                {
                    // Advance
                    previous = current;
                    current  = current.Next;
                    index++;
                }
                else if (current.Item != targetAnnotation)
                {
                    // Remove
                    if (index > ownedCount)
                        previous = shared = CopyShared(shared, current);
                    current = Link(previous, current.Next);
                    if (index < ownedCount)
                        ownedCount--;
                    break;
                }
                else
                {
                    // Same annotation already present; nothing to do
                    return;
                }
            }

            // Insert
            traits = Cell.Cons(targetAnnotation, traits);
            ownedCount++;
        }
    }
}
