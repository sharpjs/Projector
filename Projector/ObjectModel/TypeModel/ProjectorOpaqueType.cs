namespace Projector.ObjectModel
{
    using System;

    public sealed class ProjectorOpaqueType : ProjectionType
    {
        internal ProjectorOpaqueType(Type type, ProjectionFactory factory)
            : base(type, TypeKind.Opaque, factory) { }
    }
}
