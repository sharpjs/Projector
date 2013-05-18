namespace Projector.Fakes.WithSharedTraitSpec
{
    public static class Traits
    {
        private const string
            SharedTraitsPrefix = "A/Fakes/WithSharedTraitSpec/SharedTraits -> ",
            OtherTraitsPrefix  = "A/Fakes/WithSharedTraitSpec/OtherTraits -> ";

        public const string
            TypeA           = SharedTraitsPrefix + "TypeA",
            TypeB           = SharedTraitsPrefix + "TypeB",
            PropertyA       = SharedTraitsPrefix + "PropertyA",
            PropertyB       = SharedTraitsPrefix + "PropertyB",

            OtherTypeA      = OtherTraitsPrefix  + "TypeA",
            OtherTypeB      = OtherTraitsPrefix  + "TypeB",
            OtherPropertyA  = OtherTraitsPrefix  + "PropertyA",
            OtherPropertyB  = OtherTraitsPrefix  + "PropertyB";
    }
}
