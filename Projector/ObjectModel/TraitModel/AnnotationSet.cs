namespace Projector.ObjectModel
{
    using System;

    internal sealed class AnnotationSet : TraitSet<object>
    {
        internal override void Apply(object trait)
        {
            if (traits == null)
            {
                // Add first annotation
                traits = Cell.Cons(trait);
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
        }

        private void MergeAnnotationAllowSingle(object targetAnnotation)
        {
            var current    = traits;
            var previous   = null as Cell<object>;
            var targetType = targetAnnotation.GetType();

            while (current != null)
            {
                if (current.Item.GetType() != targetType)
                {
                    // Advance
                    previous = current;
                    current  = current.Next;
                }
                else if (current.Item != targetAnnotation)
                {
                    // Remove
                    Link(previous, current.Next);
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
        }
    }
}
