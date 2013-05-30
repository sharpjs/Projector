namespace Projector
{
    partial class AssemblyA
    {
        partial class Fakes
        {
            public static class WithManyTraitSpecs
            {
                private const string
                    TraitPath = Fakes.TraitPath + ".WithManyTraitSpecs";

                public static class TypeATraits
                {
                    private const string
                        TraitPath = WithManyTraitSpecs.TraitPath + ".TypeATraits";

                    public const string
                        TypeA      = TraitPath + ":TypeA",
                        PropertyA  = TraitPath + ":PropertyA",
                        PropertyB  = TraitPath + ":PropertyB",
                        Properties = TraitPath + ":Properties";
                }

                public static class SharedTraits
                {
                    private const string
                        TraitPath = WithManyTraitSpecs.TraitPath + ".SharedTraits";

                    public static class TypeA
                    {
                        private const string
                            TraitPath = SharedTraits.TraitPath + ".TypeA";

                        public const string
                            Type       = TraitPath + ":Type",
                            PropertyA  = TraitPath + ":PropertyA",
                            PropertyB  = TraitPath + ":PropertyB",
                            Properties = TraitPath + ":Properties";
                    }

                    public static class Types
                    {
                        private const string
                            TraitPath = SharedTraits.TraitPath + ".Types";

                        public const string
                            Type       = TraitPath + ":Type",
                            PropertyA  = TraitPath + ":PropertyA",
                            PropertyB  = TraitPath + ":PropertyB",
                            Properties = TraitPath + ":Properties";
                    }

                    public const string
                        TypeB      = TraitPath + ":TypeB",
                        Properties = TraitPath + ":Properties";
                }

                public static class IncludedTraits
                {
                    private const string
                        TraitPath = WithManyTraitSpecs.TraitPath + ".IncludedTraits";

                    public const string
                        TypeA      = TraitPath + ":TypeA",
                        PropertyA  = TraitPath + ":PropertyA",
                        PropertyB  = TraitPath + ":PropertyB",
                        Properties = TraitPath + ":Properties";
                }

                public static class OtherTraits
                {
                    private const string
                        TraitPath = WithManyTraitSpecs.TraitPath + ".OtherTraits";

                    public const string
                        Types      = TraitPath + ":Types",
                        Properties = TraitPath + ":Properties";
                }
            }
        }
    }
}
