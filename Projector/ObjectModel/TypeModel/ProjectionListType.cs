namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionListType : ProjectionCollectionType
    {
        internal ProjectionListType(Type type, ProjectionFactory factory)
            : base(TypeKind.List, type, factory) { }

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
