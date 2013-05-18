namespace Projector.Fakes.WithTraitSpecs
{
    using Projector.Specs;

    public class TypeATraits : TypeTraitSpec<ITypeA>
    {
        public static readonly object[]
            TypeTraits      = { new object(), new object() },
            PropertyATraits = { new object(), new object() },
            PropertyBTraits = { new object(), new object() };

        public TypeATraits()
        {
        }
    }
}
