namespace Projector.Fakes.WithSharedTraitSpec
{
    using Projector.Specs;

    public class SharedTraits : SharedTraitSpec
    {
        public SharedTraits()
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
