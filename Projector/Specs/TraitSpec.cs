namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;

    public abstract class TraitSpec
    {
        internal TraitSpec() { }

        internal abstract TypeTraitSpec SpecializeFor(Type type);

        internal static void Collect(List<object> traits, ITraitAggregator aggregator)
        {
            if (traits != null)
                foreach (var trait in traits)
                    aggregator.Collect(trait);
        }
    }
}
