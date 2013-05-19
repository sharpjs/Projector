namespace Projector
{
    partial class AssemblyB
    {
        partial class Fakes
        {
            public static class WithTypeTraitSpec
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".WithTypeTraitSpec";

                public static class TypeATraits
                {
                    private const string
                        TraitPath = WithTypeTraitSpec.TraitPath + ".TypeATraits";

                    public const string
                        TypeA     = TraitPath + ":TypeA",
                        PropertyA = TraitPath + ":PropertyA",
                        PropertyB = TraitPath + ":PropertyB";
                }

                public static class OtherTraits
                {
                    private const string
                        TraitPath = WithTypeTraitSpec.TraitPath + ".OtherTraits";

                    public const string
                        TypeA     = TraitPath + ":TypeA",
                        TypeB     = TraitPath + ":TypeB",
                        PropertyA = TraitPath + ":PropertyA",
                        PropertyB = TraitPath + ":PropertyB";
                }
            }
        }
    }
}
