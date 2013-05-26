namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class TypePredicateRestriction
        : PredicateRestriction<ProjectionType>, ITypeRestriction
    {
        public TypePredicateRestriction(Func<ProjectionType, bool> predicate, string description)
            : base(predicate, description) { }
    }
}
