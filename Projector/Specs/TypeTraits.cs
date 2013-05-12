namespace Projector.Specs
{
    using System;
    using System.Linq.Expressions;
    using Projector.ObjectModel;

    public abstract class TypeTraits<T> : TraitSpec
    {
        private readonly TypeScope<T> scope;

        internal TypeTraits()
        {
            scope = new TypeScope<T>();
        }

        protected IPropertyCut Properties
        {
            get { return scope.Properties; }
        }

        protected IPropertyScope Property(string name)
        {
            return scope.Property(name);
        }

        protected IPropertyScope Property(Expression<Func<T, object>> expression)
        {
            return scope.Property(expression);
        }

        protected ITraitScope Apply(object trait)
        {
            return scope.Apply(trait);
        }

        protected ITraitScope Apply(Func<object> factory)
        {
            return scope.Apply(factory);
        }

        internal override void CollectRelevantScopes(
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator aggregator)
        {
            if (underlyingType == typeof(T))
                aggregator.Add(scope);
        }
    }
}
