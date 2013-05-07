namespace Projector
{
    using System.Reflection;

    public interface ITraitResolution
    {
        void ResolveTypeTraits(ITraitAggregator aggregator);

        void ResolvePropertyTraits(PropertyInfo property, ITraitAggregator aggregator);
    }
}
