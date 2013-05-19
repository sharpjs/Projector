namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.OtherTraits;

    public class OtherTraits : SharedTraitSpec
    {
        public OtherTraits()
        {
            Type<WithNoTraitSpecs.ITypeA>().Spec(t =>
            {
                t.Apply(Traits.TypeA);
                t.Property(o => o.PropertyA).Apply(Traits.PropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.PropertyB);
            });

            Type<WithNoTraitSpecs.ITypeB>().Apply(Traits.TypeB);
        }
    }
}
