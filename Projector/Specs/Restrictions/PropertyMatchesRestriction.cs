namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class PropertyMatchesRestriction : IPropertyRestriction
    {
        private readonly Func<ProjectionProperty, bool> predicate;

        public PropertyMatchesRestriction(Func<ProjectionProperty, bool> predicate)
        {
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            this.predicate = predicate;
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return predicate(property);
        }

        public override string ToString()
        {
            return "matches predicate";
        }
    }
}
