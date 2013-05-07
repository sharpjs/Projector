namespace Projector
{
    using System;

    public interface ITraitResolver
    {
        ITraitResolution Resolve(Type type);
    }
}
