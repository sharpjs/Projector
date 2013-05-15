namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    public abstract class TraitSpec
    {
        internal TraitSpec() { }

        internal abstract void ProvideScopes
        (
            ProjectionType       projectionType,
            Type                 underlyingType,
            ITypeScopeAggregator action
        );
    }
}
