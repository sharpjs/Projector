namespace Projector.ObjectModel
{
    using System;

    internal abstract class ProjectionCollectionType : ProjectionType
    {
        private readonly ProjectionType keyType;
        private readonly ProjectionType itemType;

        internal ProjectionCollectionType(TypeKind kind, Type type, ProjectionFactory factory)
            : base(type, kind, factory)
        {
            // Must register before getting key/item types, due to possible cycles
            // Example: A -> IList<B> -> B -> IList<B>
            factory.RegisterProjectionType(this);

            Type keyType, itemType;
            GetSubtypes(out keyType, out itemType);

            this.keyType  = factory.GetProjectionTypeUnsafe(keyType );
            this.itemType = factory.GetProjectionTypeUnsafe(itemType);
        }

        protected abstract void GetSubtypes(out Type keyType, out Type itemType);

        public override ProjectionType CollectionKeyType
        {
            get { return keyType; }
        }

        public override ProjectionType CollectionItemType
        {
            get { return itemType; }
        }
    }
}
