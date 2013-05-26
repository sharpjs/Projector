namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class NameRestriction : ITypeRestriction, IPropertyRestriction
    {
        private readonly string           name;
        private readonly StringComparison comparison;

        public NameRestriction(string name, StringComparison comparison)
        {
            if (name == null)
                throw Error.ArgumentNull("name");

            this.name       = name;
            this.comparison = comparison;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return type.Name.Equals(name, comparison);
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

    partial class TypeCutExtensions
    {
        public static ITypeCut Named(this ITypeCut cut, string name)
        {
            return Required(cut).Matching(new NameRestriction(name, StringComparison.Ordinal));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut Named(this IPropertyCut cut, string name)
        {
            return Required(cut).Matching(new NameRestriction(name, StringComparison.Ordinal));
        }
    }
}
