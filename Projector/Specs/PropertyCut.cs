namespace Projector.Specs
{
    using System.Collections.Generic;
    using Projector.ObjectModel;

    // A collection of traits that apply to multiple properties
    internal class PropertyCut : PropertyScope, IPropertyCut
    {
        private List<IPropertyRestriction> restrictions;

        internal PropertyCut() { }

        public IPropertyCut Matching(IPropertyRestriction restriction)
        {
            if (restriction == null)
                throw Error.ArgumentNull("restriction");

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
