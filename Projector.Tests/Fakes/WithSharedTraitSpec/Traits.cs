namespace Projector
{
    partial class AssemblyB
    {
        partial class Fakes
        {
            public static class WithSharedTraitSpec
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".WithSharedTraitSpec";

                public static class SharedTraits
                {
                    private const string
                        TraitPath = WithSharedTraitSpec.TraitPath + ".SharedTraits";

                    public const string
                        TypeA     = TraitPath + ":TypeA",
                        TypeB     = TraitPath + ":TypeB",
                        PropertyA = TraitPath + ":PropertyA",
                        PropertyB = TraitPath + ":PropertyB";
                }

                public static class OtherTraits
                {
                    private const string
                        TraitPath = WithSharedTraitSpec.TraitPath + ".OtherTraits";

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
