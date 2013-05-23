namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class TypeMatchesRestriction : ITypeRestriction
    {
        private readonly Func<ProjectionType, bool> predicate;

        public TypeMatchesRestriction(Func<ProjectionType, bool> predicate)
        {
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            this.predicate = predicate;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return predicate(type);
        }

        public override string ToString()
        {
            return "matches predicate";
        }
    }
}
