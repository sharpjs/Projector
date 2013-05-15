namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using Projector.ObjectModel;

    // Trait spec about any types, e.g. DomainSharedTraits
    public abstract class SharedTraits : TraitSpec
    {
        private TypeScope                   globalTypeScope;
        private List<TypeCut>               generalTypeScopes;
        private Dictionary<Type, TypeScope> specificTypeScopes;

        // Consumers implement spec in constructor
        protected SharedTraits() { }

        protected ITypeCut Types
        {
            get
            {
                var scopes = generalTypeScopes
                    ?? (generalTypeScopes = new List<TypeCut>());
                var scope = new TypeCut();
                scopes.Add(scope);
                return scope;
            }
        }

        protected IPropertyCut Properties
        {
            get
            {
                var scope = globalTypeScope
                    ?? (globalTypeScope = new TypeScope());
                return scope.Properties;
            }
        }

        protected ITypeScope<T> Type<T>()
        {
            TypeScope scope;
            var type = typeof(T);
            var scopes = specificTypeScopes
                ?? (specificTypeScopes = new Dictionary<Type, TypeScope>());
            if (!scopes.TryGetValue(type, out scope))
                scopes[type] = scope = new TypeScope<T>();
            return scope as TypeScope<T>;
        }

        internal sealed override void ProvideScopes(
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator aggregator)
        {
            ProvideGlobalScope  (                aggregator);
            ProvideGeneralScopes(projectionType, aggregator);
            ProvideSpecificScope(underlyingType, aggregator);
        }

        private void ProvideGlobalScope(ITypeScopeAggregator aggregator)
        {
            var scope = globalTypeScope;
            if (scope != null)
                aggregator.Add(scope);
        }

        private void ProvideGeneralScopes(ProjectionType type, ITypeScopeAggregator aggregator)
        {
            var scopes = generalTypeScopes;
            if (scopes != null)
                foreach (var scope in scopes)
                    if (scope.AppliesTo(type))
                        aggregator.Add(scope);
        }

        private void ProvideSpecificScope(Type type, ITypeScopeAggregator aggregator)
        {
            TypeScope scope;
            var scopes = specificTypeScopes;
            if (scopes != null && scopes.TryGetValue(type, out scope))
                aggregator.Add(scope);
        }
    }
}
