namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    // Trait spec about any types, e.g. DomainSharedTraits
    public abstract class SharedTraitSpec : TraitSpec
    {
        private readonly SharedScope scope;

        protected SharedTraitSpec()
        {
            scope = new SharedScope();
        }

        internal sealed override void Build() { Build(scope); }

        protected abstract void Build(ISharedScope scope);

        internal sealed override void ProvideScopes(
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator aggregator)
        {
            scope.ProvideScopes(projectionType, underlyingType, aggregator);
        }
    }
}
