namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.ConditionalTraits;

    public class ConditionalTraits : SharedTraitSpec
    {
        protected override void Build(ISharedScope scope)
        {
            scope.Types.Matching(t => t.Name.EndsWith("A")).Spec(t =>
            {
                t.Apply(Traits.TypeA.Type);
                t.Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.TypeA.PropertyA);
                t.Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.TypeA.PropertyB);
            });

            scope.Types.Matching(t => t.Name.EndsWith("B")).Spec(t =>
            {
                t.Apply(Traits.TypeB.Type);
                t.Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.TypeB.PropertyA);
                t.Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.TypeB.PropertyB);
            });

            scope.Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.Global.PropertyA);
            scope.Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.Global.PropertyB);
        }
    }
}
