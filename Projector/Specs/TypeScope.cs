namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Projector.ObjectModel;

    internal class TypeScope : TraitScope, ITypeScope
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
            PropertyScope scope;
            var scopes = specificPropertyScopes
                ?? (specificPropertyScopes = new Dictionary<string, PropertyScope>());
            return scopes.TryGetValue(name, out scope)
                ? scope
                : scopes[name] = new PropertyScope();
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

    internal class TypeScope<T> : TypeScope, ITypeScope<T>
    {
        internal TypeScope() { }

        public IPropertyScope Property(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw Error.ArgumentNull("property");
            if (expression.NodeType != ExpressionType.Lambda)
                throw Error.ArgumentOutOfRange("property");

            var access = expression.Body as MemberExpression;
            if (access == null)
                throw Error.ArgumentOutOfRange("property");

            var property = access.Member as PropertyInfo;
            if (property == null)
                throw Error.ArgumentOutOfRange("property");

            return Property(property.Name);
        }
    }
}
