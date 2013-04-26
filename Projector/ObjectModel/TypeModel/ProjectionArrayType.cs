namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionArrayType : ProjectionCollectionType
    {
        internal ProjectionArrayType(Type type, ProjectionFactory factory)
            : base(TypeKind.Array, type, factory) { }

        protected override void GetSubtypes(out Type keyType, out Type itemType)
        {
            keyType  = typeof(int);
            itemType = UnderlyingType.GetElementType();
        }
    }
}
