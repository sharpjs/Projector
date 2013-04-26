namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionDictionaryType : ProjectionCollectionType
    {
        internal ProjectionDictionaryType(Type type, ProjectionFactory factory)
            : base(TypeKind.Dictionary, type, factory) { }

        protected override void GetSubtypes(out Type keyType, out Type itemType)
        {
            var types = UnderlyingType.GetGenericArguments();
            keyType  = types[0];
            itemType = types[1];
        }

        public override bool IsVirtualizable
        {
            get { return true; }
        }
    }
}
