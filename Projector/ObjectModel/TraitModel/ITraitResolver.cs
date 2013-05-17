namespace Projector.ObjectModel
{
    using System;

    public interface ITraitResolver
    {
        ITraitResolution Resolve(ProjectionType projectionType, Type underlyingType);
    }
}
