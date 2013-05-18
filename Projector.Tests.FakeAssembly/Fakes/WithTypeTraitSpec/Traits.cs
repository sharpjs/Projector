namespace Projector.Fakes.WithTypeTraitSpec
{
    public static class Traits
    {
        private const string
            TypeATraitsPrefix = "A/Fakes/WithTypeTraitSpec/TypeATraits -> ",
            OtherTraitsPrefix = "A/Fakes/WithTypeTraitSpec/OtherTraits -> ";

        public const string
            TypeA          = TypeATraitsPrefix + "TypeA",
            PropertyA      = TypeATraitsPrefix + "PropertyA",
            PropertyB      = TypeATraitsPrefix + "PropertyB",

            OtherTypeA     = OtherTraitsPrefix + "TypeA",
            OtherTypeB     = OtherTraitsPrefix + "TypeB",
            OtherPropertyA = OtherTraitsPrefix + "PropertyA",
            OtherPropertyB = OtherTraitsPrefix + "PropertyB";
    }
}
