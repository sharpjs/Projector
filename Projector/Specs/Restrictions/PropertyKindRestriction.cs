namespace Projector.Specs
{
    using Projector.ObjectModel;

    internal sealed class PropertyKindRestriction : IPropertyRestriction
    {
        private readonly TypeKind kind;

        public PropertyKindRestriction(TypeKind kind)
        {
            this.kind = kind;
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
}
