namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    public abstract class TypeTraitSpec : TraitSpec
    {
        private readonly SharedTraitSpec         parentSpec;
        private                    List<object>  typeTraits;
        private                    List<object>  generalPropertyTraits;
        private Dictionary<string, List<object>> specificPropertyTraits;

        internal TypeTraitSpec() { }

        internal void ResolveTypeTraits(ITraitAggregator aggregator)
        {
            var parent = this.parentSpec;
            if (parent != null)
                parent.ResolveGeneralTypeTraits(aggregator);

            Collect(typeTraits, aggregator);
        }

        internal void ResolvePropertyTraits(PropertyInfo property, ITraitAggregator aggregator)
        {
            var parent = this.parentSpec;
            if (parent != null)
                parent.ResolveGeneralPropertyTraits(aggregator);

            Collect(generalPropertyTraits, aggregator);

            List<object> traits;
            var dictionary = specificPropertyTraits;
            if (dictionary != null && dictionary.TryGetValue(property.Name, out traits))
                Collect(traits, aggregator);
        }
    }

    // Trait spec about a specific type, e.g. CustomerTraits
    public abstract class TraitSpec<T> : TypeTraitSpec
    {
        // Consumers implement spec in constructor
        protected TraitSpec() { }

        internal override TypeTraitSpec SpecializeFor(Type type)
        {
            return type == typeof(T) ? this : null;
        }

        protected void Apply(object trait) { }
        protected void Apply(Func<ITraitContext, object> factory) { }

        protected IPropertyCut Properties { get { return null; } }

        protected ITraitScope Property(Expression<Func<T, object>>[] property) { return null; }

        protected ITraitScope Property(string name) { return null; }
    }
}
