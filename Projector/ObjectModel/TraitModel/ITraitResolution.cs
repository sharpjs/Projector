namespace Projector
{
    using System.Reflection;
    using Projector.ObjectModel;

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
