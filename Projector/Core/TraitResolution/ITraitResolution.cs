namespace Projector
{
    using System.Reflection;
    using Projector.ObjectModel;

    public interface ITraitResolution
    {
        void ResolveTypeTraits(
            ITraitAggregator   aggregator);

        void ResolvePropertyTraits(
            ProjectionProperty projectionProperty,
            PropertyInfo       underlyingProperty,
            ITraitAggregator   aggregator);
    }
}
