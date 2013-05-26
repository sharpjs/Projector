namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class PropertyPredicateRestriction
        : PredicateRestriction<ProjectionProperty>, IPropertyRestriction
    {
        public PropertyPredicateRestriction(Func<ProjectionProperty, bool> predicate, string description)
            : base(predicate, description) { }
    }
}
