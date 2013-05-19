﻿namespace Projector.Fakes.WithSharedTraitSpec
{
    using Projector.Specs;
    using Traits = AssemblyB.Fakes.WithSharedTraitSpec.OtherTraits;

    public class OtherTraits : SharedTraitSpec
    {
        public OtherTraits()
        {
            Type<ITypeA>().Spec(t =>
            {
                t.Apply(Traits.TypeA);
                t.Property(o => o.PropertyA).Apply(Traits.PropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.PropertyB);
            });

            Type<ITypeB>().Apply(Traits.TypeB);
        }
    }
}
