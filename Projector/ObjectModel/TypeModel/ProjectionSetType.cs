namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionSetType : ProjectionCollectionType
    {
        internal ProjectionSetType(Type type, ProjectionFactory factory)
            : base(TypeKind.Set, type, factory) { }

        protected override void GetSubtypes(out Type keyType, out Type itemType)
        {
            keyType  = typeof(int);
            itemType = UnderlyingType.GetGenericArguments()[0];
        }

        public override bool IsVirtualizable
        {
            get { return true; }
        }
    }
}
