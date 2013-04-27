namespace Projector.ObjectModel
{
    using System;

    internal sealed class ProjectionOpaqueType : ProjectionType
    {
        internal ProjectionOpaqueType(Type type, ProjectionFactory factory)
            : base(type, TypeKind.Opaque, factory)
        {
            factory.RegisterProjectionType(this);
        }
    }
}
