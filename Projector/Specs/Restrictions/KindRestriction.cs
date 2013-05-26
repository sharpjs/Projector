namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal sealed class KindRestriction : ITypeRestriction, IPropertyRestriction
    {
        private readonly TypeKind kind;

        public KindRestriction(TypeKind kind)
        {
            this.kind = kind;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return type.Kind == kind;
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return property.PropertyType.Kind == kind;
        }

        public override string ToString()
        {
            return string.Format("is of {0} kind", kind);
        }
    }

    partial class TypeCutExtensions
    {
        public static ITypeCut OfKind(this ITypeCut cut, TypeKind kind)
        {
            return Required(cut).Matching(new KindRestriction(kind));
        }
    }

    partial class PropertyCutExtensions
    {
        public static IPropertyCut OfKind(this IPropertyCut cut, TypeKind kind)
        {
            return Required(cut).Matching(new KindRestriction(kind));
        }
    }
}
