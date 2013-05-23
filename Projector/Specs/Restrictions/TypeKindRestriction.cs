namespace Projector.Specs
{
    using Projector.ObjectModel;

    internal sealed class TypeKindRestriction : ITypeRestriction
    {
        private readonly TypeKind kind;

        public TypeKindRestriction(TypeKind kind)
        {
            this.kind = kind;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return type.Kind == kind;
        }

        public override string ToString()
        {
            return string.Format("is of {0} kind", kind);
        }
    }
}
