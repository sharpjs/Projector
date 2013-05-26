namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal abstract class PredicateRestriction<T>
        where T : ProjectionMetaObject
    {
        private readonly Func<T, bool> predicate;
        private readonly string        description;

        protected PredicateRestriction(Func<T, bool> predicate, string description)
        {
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            this.predicate   = predicate;
            this.description = description;
        }

        public bool AppliesTo(T property)
        {
            return predicate(property);
        }

        public override string ToString()
        {
            return description ?? "matches predicate";
        }
    }

    internal sealed class PropertyPredicateRestriction
        : PredicateRestriction<ProjectionProperty>, IPropertyRestriction
    {
        public PropertyPredicateRestriction(Func<ProjectionProperty, bool> predicate, string description)
            : base(predicate, description) { }
    }

    internal sealed class TypePredicateRestriction
        : PredicateRestriction<ProjectionType>, ITypeRestriction
    {
        public TypePredicateRestriction(Func<ProjectionType, bool> predicate, string description)
            : base(predicate, description) { }
    }

    partial class TypeCutExtensions
    {
        public static ITypeCut Matching(this ITypeCut cut, Func<ProjectionType, bool> predicate)
        {
            return Required(cut).Matching(new TypePredicateRestriction(predicate, null));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut Matching(this IPropertyCut cut, Func<ProjectionProperty, bool> predicate)
        {
            return Required(cut).Matching(new PropertyPredicateRestriction(predicate, null));
        }
    }
}
