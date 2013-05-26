namespace Projector.Specs
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class KindRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void ToStringMethod()
        {
            var restriction = new KindRestriction(TypeKind.List);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("is of List kind"));
        }
    }
}
