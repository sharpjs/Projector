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
}
