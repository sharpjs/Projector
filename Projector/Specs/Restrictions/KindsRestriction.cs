namespace Projector.Specs
{
    using System.Text;
    using Projector.ObjectModel;

    internal sealed class KindsRestriction : ITypeRestriction, IPropertyRestriction
    {
        private readonly TypeKind[] kinds;

        public KindsRestriction(TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");

            this.kinds = kinds;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return HasKind(type.Kind);
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return HasKind(property.PropertyType.Kind);
        }

        private bool HasKind(TypeKind candidate)
        {
            foreach (var kind in kinds)
                if (kind == candidate)
                    return true;

            return false;
        }

        public override string ToString()
        {
            return new StringBuilder("is of any kind in ")
                .AppendList(kinds)
                .ToString();
        }
    }

    partial class TypeCutExtensions
    {
        public static ITypeCut OfKind(this ITypeCut cut, params TypeKind[] kinds)
        {
            return Required(cut).Matching(new KindsRestriction(kinds));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut OfKind(this IPropertyCut cut, params TypeKind[] kinds)
        {
            return Required(cut).Matching(new KindsRestriction(kinds));
        }
    }
}
