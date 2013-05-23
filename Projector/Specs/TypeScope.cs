namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Projector.ObjectModel;

    internal class TypeScope : TraitScope, ITypeBlock
    {
        private List<PropertyCut>                 generalPropertyScopes;
        private Dictionary<string, PropertyScope> specificPropertyScopes;

        internal TypeScope() { }

        public IPropertyCut Properties
        {
            get
            {
                var scopes = generalPropertyScopes
                    ?? (generalPropertyScopes = new List<PropertyCut>());
                var scope  = new PropertyCut();
                scopes.Add(scope);
                return scope;
            }
        }

        public IPropertyScope Property(string name)
        {
            if (name == null)
                throw Error.ArgumentNull("name");

            PropertyScope scope;
            var scopes = specificPropertyScopes
                ?? (specificPropertyScopes = new Dictionary<string, PropertyScope>());
            return scopes.TryGetValue(name, out scope)
                ? scope
                : scopes[name] = new PropertyScope();
        }

        public void Spec(Action<ITypeScope> spec)
        {
            if (spec == null)
                throw Error.ArgumentNull("spec");

            spec(this);
        }

        internal void ProvideTraits(ProjectionProperty property, ITraitAggregator aggregator)
        {
            ProvideGeneralTraits (property, aggregator);
            ProvideSpecificTraits(property, aggregator);
        }

        private void ProvideGeneralTraits(ProjectionProperty property, ITraitAggregator aggregator)
        {
            var scopes = generalPropertyScopes;
            if (scopes != null)
                foreach (var scope in scopes)
                    if (scope.AppliesTo(property))
                        scope.ProvideTraits(aggregator);
        }

        private void ProvideSpecificTraits(ProjectionProperty property, ITraitAggregator aggregator)
        {
            PropertyScope scope;
            var scopes = specificPropertyScopes;
            if (scopes != null && scopes.TryGetValue(property.Name, out scope))
                scope.ProvideTraits(aggregator);
        }
    }

    internal class TypeScope<T> : TypeScope, ITypeBlock<T>
    {
        internal TypeScope() { }

        public IPropertyScope Property(Expression<Func<T, object>> expression)
        {
            return Property(expression.ToProperty().Name);
        }

        public void Spec(Action<ITypeScope<T>> spec)
        {
            if (spec == null)
                throw Error.ArgumentNull("spec");

            spec(this);
        }
    }
}
