namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;

    // Trait spec about any types, e.g. DomainSharedTraits
    public abstract class SharedTraitSpec : TraitSpec
    {
        private List<object>                    generalTypeTraits;
        private List<object>                    generalPropertyTraits;
        private Dictionary<Type, TypeTraitSpec> typeSpecs;

        // Consumers implement spec in constructor
        protected SharedTraitSpec() { }

        protected ITypeCut Types { get { return null; } }

        protected ITypeScope Type<T>() { return null; }

        protected IPropertyCut Properties { get { return null; } }

        internal override TypeTraitSpec SpecializeFor(Type type)
        {
            return null;
        }

        internal void ResolveGeneralTypeTraits(ITraitAggregator aggregator)
        {
            Collect(generalTypeTraits, aggregator);
        }

        internal void ResolveGeneralPropertyTraits(ITraitAggregator aggregator)
        {
            Collect(generalPropertyTraits, aggregator);
        }
    }
}
