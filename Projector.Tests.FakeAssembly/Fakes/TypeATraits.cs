namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.TypeATraits;

    public class TypeATraits : TypeTraitSpec<WithNoTraitSpecs.ITypeA>
    {
        protected override void Build(ITypeScope<WithNoTraitSpecs.ITypeA> scope)
        {
            scope.Apply(Traits.TypeA);
            scope.Property(o => o.PropertyA).Apply(Traits.PropertyA);
            scope.Property(o => o.PropertyB).Apply(Traits.PropertyB);
        }
    }
}
