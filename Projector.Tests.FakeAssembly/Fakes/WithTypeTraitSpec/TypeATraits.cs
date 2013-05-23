namespace Projector.Fakes.WithTypeTraitSpec
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits;

    public class TypeATraits : TypeTraitSpec<ITypeA>
    {
        protected override void Build(ITypeScope<ITypeA> scope)
        {
            scope.Apply(Traits.TypeA);
            scope.Property(o => o.PropertyA).Apply(Traits.PropertyA);
            scope.Property(o => o.PropertyB).Apply(Traits.PropertyB);
        }
    }
}
