namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public abstract class TypeTraitSpec<T> : TraitSpec
    {
        private readonly TypeScope<T> scope;

        protected TypeTraitSpec()
        {
            scope = new TypeScope<T>();
        }

        internal override void Build() { Build(scope); }

        protected abstract void Build(ITypeScope<T> scope);

        internal sealed override void ProvideScopes(
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator aggregator)
        {
            if (underlyingType == typeof(T))
                aggregator.Add(scope);
        }
    }
}
