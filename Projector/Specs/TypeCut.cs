namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Projector.ObjectModel;

    // A collection of traits that apply multiple types
    internal class TypeCut : TypeScope, ITypeCut
    {
        private List<ITypeRestriction> restrictions;

        internal TypeCut() { }

        public ITypeCut Named(string name)
        {
            return Restrict(new NameRestriction(name, StringComparison.Ordinal));
        }

        public ITypeCut Named(params string[] names)
        {
            return Restrict(new NamesRestriction(names, StringComparison.Ordinal));
        }

        public ITypeCut NamedLike(string pattern)
        {
            return Restrict(new NameRegexRestriction(pattern, RegexOptions.CultureInvariant));
        }

        public ITypeCut OfKind(TypeKind kind)
        {
            return Restrict(new KindRestriction(kind));
        }

        public ITypeCut OfKind(params TypeKind[] kinds)
        {
            return Restrict(new KindsRestriction(kinds));
        }

        public ITypeCut Matching(Func<ProjectionType, bool> predicate)
        {
            return Restrict(new TypePredicateRestriction(predicate, null));
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
