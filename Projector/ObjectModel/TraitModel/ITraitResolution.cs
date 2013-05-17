namespace Projector.ObjectModel
{
    using System.Reflection;

    public interface ITraitResolution
    {
        void ProvideTypeTraits(
            ITraitAggregator   aggregator);

        void ProvidePropertyTraits(
            ProjectionProperty projectionProperty,
            PropertyInfo       underlyingProperty,
            ITraitAggregator   aggregator);
    }
}
