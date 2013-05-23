namespace Projector.Specs
{
    using System.Linq;
    using System.Text;
    using Projector.ObjectModel;

    internal sealed class TypeKindsRestriction : ITypeRestriction
    {
        private readonly TypeKind[] kinds;

        public TypeKindsRestriction(TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");

            this.kinds = kinds;
        }

        public bool AppliesTo(ProjectionType type)
        {
            return kinds.Contains(type.Kind);
        }

        public override string ToString()
        {
            var text = new StringBuilder()
                .Append("is of any kind in [");

            foreach (var kind in kinds)
            {
                if (text.Length != 0)
                    text.Append(", ");
                text.Append(kind);
            }

            return text.Append(']').ToString();
        }
    }
}
