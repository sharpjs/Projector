namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;

    internal class TraitScope : ITraitScope
    {
        private List<object> traits;

        public ITraitScope Apply(object trait)
        {
            return Add(trait);
        }

        public ITraitScope Apply(Func<ITraitContext, object> factory)
        {
            return Add(factory);
        }

        private ITraitScope Add(object trait)
        {
            var traits = this.traits;
            if (traits == null)
                this.traits = traits = new List<object>();
            traits.Add(trait);
            return this;
        }

        internal virtual void Collect(ITraitAggregator aggregator)
        {
            TraitSpec.Collect(traits, aggregator);
        }
    }
}
