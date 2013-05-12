namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Projector.ObjectModel;

    // A collection of traits that apply to multiple properties
    internal class PropertyCut : PropertyScope, IPropertyCut
    {
        private List<Func<ProjectionProperty, bool>> predicates;

        internal PropertyCut() { }

        public IPropertyCut OfKind(TypeKind kind)
        {
            return Restrict(p => p.PropertyType.Kind == kind);
        }

        public IPropertyCut OfKind(params TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");

            return Restrict(p => kinds.Contains(p.PropertyType.Kind));
        }

        public IPropertyCut Named(string name)
        {
            return Restrict(p => p.Name.Equals(name));
        }

        public IPropertyCut Named(params string[] names)
        {
            if (names == null)
                throw Error.ArgumentNull("names");

            return Restrict(p => names.Contains(p.Name));
        }

        public IPropertyCut Matching(Func<ProjectionProperty, bool> predicate)
        {
            if (predicate == null)
                throw Error.ArgumentNull("predicate");

            return Restrict(predicate);
        }

        protected PropertyCut Restrict(Func<ProjectionProperty, bool> predicate)
        {
            (predicates ?? (predicates = new List<Func<ProjectionProperty, bool>>()))
                .Add(predicate);
            return this;
        }

        internal bool AppliesTo(ProjectionProperty property)
        {
            var predicates = this.predicates;
            if (predicates != null)
                foreach (var predicate in predicates)
                    if (!predicate(property))
                        return false;

            return true;
        }
    }
}
