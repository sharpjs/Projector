namespace Projector.Fakes.WithSharedTraitSpec
{
    using Projector.Specs;

    public class OtherTraits : SharedTraitSpec
    {
        public OtherTraits()
        {
            Type<ITypeA>().Spec(t =>
            {
                t.Apply(Traits.OtherTypeA);
                t.Property(o => o.PropertyA).Apply(Traits.OtherPropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.OtherPropertyB);
            });

            Type<ITypeB>().Apply(Traits.OtherTypeB);
        }
    }
}
