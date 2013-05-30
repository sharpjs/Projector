namespace Projector.Fakes.WithManyTraitSpecs
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits;

    public class SharedTraits : SharedTraitSpec
    {
        protected override void Build(ISharedScope scope)
        {
            scope.Type<ITypeA>().Spec(t =>
            {
                t.Apply(Traits.TypeA.Type);
                t.Property(o => o.PropertyA).Apply(Traits.TypeA.PropertyA);
                t.Property(o => o.PropertyB).Apply(Traits.TypeA.PropertyB);
                t.Properties.Apply(Traits.TypeA.Properties);
            });

            scope.Type<ITypeB>().Apply(Traits.TypeB);

            scope.Types.Spec(t =>
            {
                t.Apply(Traits.Types.Type);
                t.Property("PropertyA").Apply(Traits.Types.PropertyA);
                t.Property("PropertyB").Apply(Traits.Types.PropertyB);
                t.Properties.Apply(Traits.Types.Properties);
            });

            scope.Properties.Apply(Traits.Properties);
        }
    }
}
