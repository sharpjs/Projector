namespace Projector.Specs
{
    using System.Linq;
    using System.Text;
    using Projector.ObjectModel;

    internal sealed class PropertyKindsRestriction : IPropertyRestriction
    {
        private readonly TypeKind[] kinds;

        public PropertyKindsRestriction(TypeKind[] kinds)
        {
            if (kinds == null)
                throw Error.ArgumentNull("kinds");

            this.kinds = kinds;
        }

        public bool AppliesTo(ProjectionProperty property)
        {
            return kinds.Contains(property.PropertyType.Kind);
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
