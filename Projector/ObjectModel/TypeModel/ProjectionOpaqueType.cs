namespace Projector.ObjectModel
{
    using System;

    public sealed class ProjectionOpaqueType : ProjectionType
    {
        internal ProjectionOpaqueType(Type type, ProjectionFactory factory)
            : base(type, TypeKind.Opaque, factory)
        {
            factory.RegisterProjectionType(this);
        }
    }
}
