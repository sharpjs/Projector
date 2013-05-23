namespace Projector.Specs
{
    using System;
    using Projector.ObjectModel;

    internal class FakeTraitSpec : TraitSpec
    {
        internal sealed override void ProvideScopes(
            ProjectionType projectionType, Type underlyingType, ITypeScopeAggregator action)
        {
            // Stub only
        }

        internal sealed override void Build()
        {
            // Stub only
        }
    }

    internal class FakeTraitSpecA : FakeTraitSpec { }

    internal class FakeTraitSpecB : FakeTraitSpec { }
}
