namespace Projector
{
    using System;
    using Projector.ObjectModel;

    public interface ITraitResolver
    {
        ITraitResolution Resolve(ProjectionType projectionType, Type underlyingType);
    }
}
