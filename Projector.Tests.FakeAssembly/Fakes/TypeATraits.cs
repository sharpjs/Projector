﻿namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.TypeATraits;

    public class TypeATraits : TypeTraitSpec<WithNoTraitSpecs.ITypeA>
    {
        public TypeATraits()
        {
            Apply(Traits.TypeA);
            Property(o => o.PropertyA).Apply(Traits.PropertyA);
            Property(o => o.PropertyB).Apply(Traits.PropertyB);
        }
    }
}