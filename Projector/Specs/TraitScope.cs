namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;

    // A collection of traits that can be passed to an aggregator
    internal class TraitScope : ITraitScope
    {
        private readonly List<object> traits;

        internal TraitScope()
        {
            traits = new List<object>();
        }

        public ITraitScope Apply(object trait)
        {
            if (trait == null)
                throw Error.ArgumentNull("trait");

            traits.Add(trait);
            return this;
        }

        public ITraitScope Apply(Func<object> factory)
        {
            if (factory == null)
                throw Error.ArgumentNull("factory");

            traits.Add(factory);
            return this;
        }

        internal void ProvideTraits(ITraitAggregator aggregator)
        {
            foreach (var trait in traits)
                aggregator.Add(Realize(trait));
        }

        private static object Realize(object obj)
        {
            var factory = obj as Func<object>;
            if (factory == null)
                return obj;
            if ((obj = factory()) != null)
                return obj;
            throw Error.TraitFactoryReturnedNull();
        }
    }
}
