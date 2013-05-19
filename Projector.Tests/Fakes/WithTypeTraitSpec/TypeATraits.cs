namespace Projector.Fakes.WithTypeTraitSpec
{
    using Projector.Specs;
    using Traits = AssemblyB.Fakes.WithTypeTraitSpec.TypeATraits;

    public class TypeATraits : TypeTraitSpec<ITypeA>
    {
        public TypeATraits()
        {
            Apply(Traits.TypeA);
            Property(o => o.PropertyA).Apply(Traits.PropertyA);
            Property(o => o.PropertyB).Apply(Traits.PropertyB);
        }
    }
}
