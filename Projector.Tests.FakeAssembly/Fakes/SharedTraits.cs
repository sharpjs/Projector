namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.SharedTraits;

    public class SharedTraits : SharedTraitSpec
    {
        protected override void Build(ISharedScope scope)
        {
            scope.Type<WithNoTraitSpecs.ITypeA>().Spec(t =>
            {
                t.Apply(Traits.TypeA);
                t.Property(o => o.PropertyA).Apply(Traits.PropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.PropertyB);
            });

            scope.Type<WithNoTraitSpecs.ITypeB>().Apply(Traits.TypeB);
        }
    }
}
