namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using Projector.ObjectModel;

    // A collection of traits that apply multiple types
    internal class TypeCut : TypeScope, ITypeCut
    {
        private List<ITypeRestriction> restrictions;

        internal TypeCut() { }

        public ITypeCut OfKind(TypeKind kind)
        {
            return Restrict(new TypeKindRestriction(kind));
        }

        public ITypeCut OfKind(params TypeKind[] kinds)
        {
            return Restrict(new TypeKindsRestriction(kinds));
        }

        public ITypeCut Matching(Func<ProjectionType, bool> predicate)
        {
            return Restrict(new TypeMatchesRestriction(predicate));
        }

        public ITypeCut Matching(ITypeRestriction restriction)
        {
            if (restriction == null)
                throw Error.ArgumentNull("restriction");

            return Restrict(restriction);
        }

        protected TypeCut Restrict(ITypeRestriction restriction)
        {
            (restrictions ?? (restrictions = new List<ITypeRestriction>()))
                .Add(restriction);

            return this;
        }

        internal bool AppliesTo(ProjectionType type)
        {
            var restrictions = this.restrictions;
            if (restrictions != null)
                foreach (var restriction in restrictions)
                    if (!restriction.AppliesTo(type))
                        return false;

            return true;
        }
    }
}
