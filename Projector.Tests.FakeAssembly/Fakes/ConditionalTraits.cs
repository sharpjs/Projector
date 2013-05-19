namespace Projector.Fakes
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.ConditionalTraits;

    public class ConditionalTraits : SharedTraitSpec
    {
        public ConditionalTraits()
        {
            Types.Matching(t => t.Name.EndsWith("A")).Spec(s =>
            {
                s.Apply(Traits.TypeA.Type);
                s.Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.TypeA.PropertyA);
                s.Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.TypeA.PropertyB);
            });

            Types.Matching(t => t.Name.EndsWith("B")).Spec(s =>
            {
                s.Apply(Traits.TypeB.Type);
                s.Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.TypeB.PropertyA);
                s.Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.TypeB.PropertyB);
            });

            Properties.Matching(p => p.Name.EndsWith("A")).Apply(Traits.Global.PropertyA);
            Properties.Matching(p => p.Name.EndsWith("B")).Apply(Traits.Global.PropertyB);
        }
    }
}
