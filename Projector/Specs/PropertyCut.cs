namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Projector.ObjectModel;

    // A collection of traits that apply to multiple properties
    internal class PropertyCut : PropertyScope, IPropertyCut
    {
        private List<IPropertyRestriction> restrictions;

        internal PropertyCut() { }

        public IPropertyCut OfKind(TypeKind kind)
        {
            return Restrict(new PropertyKindRestriction(kind));
        }

        public IPropertyCut OfKind(params TypeKind[] kinds)
        {
            return Restrict(new PropertyKindsRestriction(kinds));
        }

        public IPropertyCut Named(string name)
        {
            return Restrict(new PropertyNameRestriction(name, StringComparison.Ordinal));
        }

        public IPropertyCut Named(params string[] names)
        {
            // TODO: Fix
            return Restrict(new PropertyMatchesRestriction(p => names.Contains(p.Name)));
        }

        public IPropertyCut Matching(Func<ProjectionProperty, bool> predicate)
        {
            return Restrict(new PropertyMatchesRestriction(predicate));
        }

        public IPropertyCut Matching(IPropertyRestriction restriction)
        {
            if (restriction == null)
                throw Error.ArgumentNull("restriction");

            return Restrict(restriction);
        }

        protected PropertyCut Restrict(IPropertyRestriction restriction)
        {
            (restrictions ?? (restrictions = new List<IPropertyRestriction>()))
                .Add(restriction);

            return this;
        }

        internal bool AppliesTo(ProjectionProperty property)
        {
            var restrictions = this.restrictions;
            if (restrictions != null)
                foreach (var restriction in restrictions)
                    if (!restriction.AppliesTo(property))
                        return false;

            return true;
        }
    }
}
