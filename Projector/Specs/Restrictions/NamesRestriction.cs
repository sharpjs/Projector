namespace Projector.Specs
{
    using System;
    using System.Text;
    using Projector.ObjectModel;

    internal sealed class NamesRestriction : ITypeRestriction, IPropertyRestriction
    {
        private readonly string[]         names;
        private readonly StringComparison comparison;

        public NamesRestriction(string[] names, StringComparison comparison)
        {
            if (names == null)
                throw Error.ArgumentNull("names");

            this.names      = names;
            this.comparison = comparison;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return AppliesTo(type.Name);
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return AppliesTo(property.Name);
        }

        private bool AppliesTo(string candidate)
        {
            foreach (var name in names)
                if (candidate.Equals(name, comparison))
                    return true;

            return false;
        }

        public override string ToString()
        {
            return new StringBuilder("name equals any in ")
                .AppendList(names)
                .Append(" via ")
                .Append(comparison)
                .Append(" comparison")
                .ToString();
        }
    }

    partial class TypeCutExtensions
    {
        public static ITypeCut Named(this ITypeCut cut, params string[] names)
        {
            return Required(cut).Matching(new NamesRestriction(names, StringComparison.Ordinal));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut Named(this IPropertyCut cut, params string[] names)
        {
            return Required(cut).Matching(new NamesRestriction(names, StringComparison.Ordinal));
        }
    }
}
