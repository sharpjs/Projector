namespace Projector
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projector.Specs;

    internal class StandardTraitResolution : ITraitResolution
    {
        private readonly Type       type;
        private List<TypeTraitSpec> specs;

        internal StandardTraitResolution(Type type)
        {
            this.type = type;
        }

        internal void Add(TraitSpec spec)
        {
            var actual = spec.SpecializeFor(type);
            if (actual != null)
            {
                if (specs == null)
                    specs = new List<TypeTraitSpec>();
                specs.Add(actual);
            }
        }

        public void ResolveTypeTraits(ITraitAggregator aggregator)
        {
            if (specs != null)
                foreach (var spec in specs)
                    spec.ResolveTypeTraits(aggregator);

            CollectAttributes(type, aggregator);
        }

        public void ResolvePropertyTraits(PropertyInfo property, ITraitAggregator aggregator)
        {
            if (specs != null)
                foreach (var spec in specs)
                    spec.ResolvePropertyTraits(property, aggregator);

            CollectAttributes(property, aggregator);
        }

        private static void CollectAttributes(MemberInfo source, ITraitAggregator aggregator)
        {
            foreach (var trait in source.GetCustomAttributes(false))
                aggregator.Collect(trait);
        }
    }
}
