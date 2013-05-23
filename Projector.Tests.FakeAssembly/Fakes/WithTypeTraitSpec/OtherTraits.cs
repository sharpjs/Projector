﻿namespace Projector.Fakes.WithTypeTraitSpec
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.WithTypeTraitSpec.OtherTraits;

    public class OtherTraits : SharedTraitSpec
    {
        protected override void Build(ISharedScope scope)
        {
            scope.Type<ITypeA>().Spec(t =>
            {
                t.Apply(Traits.TypeA);
                t.Property(o => o.PropertyA).Apply(Traits.PropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.PropertyB);
            });

            scope.Type<ITypeB>().Apply(Traits.TypeB);
        }
    }
}
