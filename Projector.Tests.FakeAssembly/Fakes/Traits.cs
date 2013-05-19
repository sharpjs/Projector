namespace Projector
{
    partial class AssemblyA
    {
        partial class Fakes
        {
            public static class TypeATraits
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".TypeATraits";

                public const string
                    TypeA     = TraitPath + ":TypeA",
                    PropertyA = TraitPath + ":PropertyA",
                    PropertyB = TraitPath + ":PropertyB";
            }

            public static class SharedTraits
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".SharedTraits";

                public const string
                    TypeA     = TraitPath + ":TypeA",
                    TypeB     = TraitPath + ":TypeB",
                    PropertyA = TraitPath + ":PropertyA",
                    PropertyB = TraitPath + ":PropertyB";
            }

            public static class OtherTraits
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".OtherTraits";

                public const string
                    TypeA     = TraitPath + ":TypeA",
                    TypeB     = TraitPath + ":TypeB",
                    PropertyA = TraitPath + ":PropertyA",
                    PropertyB = TraitPath + ":PropertyB";
            }

            public static class ConditionalTraits
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".ConditionalTraits";

                public static class TypeA
                {
                    private const string
                        TraitPath = ConditionalTraits.TraitPath + ".TypeA";

                    public const string
                        Type      = TraitPath,
                        PropertyA = TraitPath + ".PropertyA",
                        PropertyB = TraitPath + ".PropertyB";
                }

                public static class TypeB
                {
                    private const string
                        TraitPath = ConditionalTraits.TraitPath + ".TypeB";

                    public const string
                        Type      = TraitPath,
                        PropertyA = TraitPath + ".PropertyA",
                        PropertyB = TraitPath + ".PropertyB";
                }

                public static class Global
                {
                    private const string
                        TraitPath = ConditionalTraits.TraitPath + ".Global";

                    public const string
                        PropertyA = TraitPath + ".PropertyA",
                        PropertyB = TraitPath + ".PropertyB";
                }
            }
        }
    }
}
