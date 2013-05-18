namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Projector.ObjectModel;

    // A collection of traits that apply multiple types
    internal class TypeCut : TypeScope, ITypeCut
    {
        private List<Func<ProjectionType, bool>> predicates;

        internal TypeCut() { }

        public ITypeCut OfKind(TypeKind kind)
        {
            return Restrict(t => t.Kind == kind);
        }

        public ITypeCut OfKind(params TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");

            return Restrict(t => kinds.Contains(t.Kind));
        }

        public ITypeCut Matching(Func<ProjectionType, bool> predicate)
        {
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            return Restrict(predicate);
        }

        protected TypeCut Restrict(Func<ProjectionType, bool> predicate)
        {
            (predicates ?? (predicates = new List<Func<ProjectionType, bool>>()))
                .Add(predicate);
            return this;
        }

        internal bool AppliesTo(ProjectionType type)
        {
            var predicates = this.predicates;
            if (predicates != null)
                foreach (var predicate in predicates)
                    if (!predicate(type))
                        return false;

            return true;
        }
    }
}
