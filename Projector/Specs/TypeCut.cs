namespace Projector.Specs
{
    using System.Collections.Generic;
    using Projector.ObjectModel;

    // A collection of traits that apply multiple types
    internal class TypeCut : TypeScope, ITypeCut
    {
        private List<ITypeRestriction> restrictions;

        internal TypeCut() { }

        public ITypeCut Matching(ITypeRestriction restriction)
        {
            if (restriction == null)
                throw Error.ArgumentNull("restriction");

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
