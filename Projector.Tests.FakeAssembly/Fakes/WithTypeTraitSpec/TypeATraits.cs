namespace Projector.Fakes.WithTypeTraitSpec
{
    using Projector.Specs;

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
