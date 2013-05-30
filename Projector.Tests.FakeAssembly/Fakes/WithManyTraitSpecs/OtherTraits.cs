namespace Projector.Fakes.WithManyTraitSpecs
{
    using Projector.Specs;
    using Traits = AssemblyA.Fakes.WithManyTraitSpecs.OtherTraits;

    public class OtherTraits : SharedTraitSpec
    {
        protected override void Build(ISharedScope scope)
        {
            scope.Types     .Apply(Traits.Types     );
            scope.Properties.Apply(Traits.Properties);
        }
    }
}
