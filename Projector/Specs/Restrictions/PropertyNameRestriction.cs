namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class PropertyNameRestriction : IPropertyRestriction
    {
        private readonly string           name;
        private readonly StringComparison comparison;

        public PropertyNameRestriction(string name, StringComparison comparison)
        {
            this.name       = name;
            this.comparison = comparison;
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return property.Name.Equals(name, comparison);
        }

        public override string ToString()
        {
            return string.Format
            (
                "name equals '{0}' via {1} comparison",
                name, comparison
            );
        }
    }
}
