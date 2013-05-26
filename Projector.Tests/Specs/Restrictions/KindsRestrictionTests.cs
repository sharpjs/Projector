namespace Projector.Specs
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class KindsRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void ToStringMethod()
        {
            var restriction = new KindsRestriction(new[] { TypeKind.List, TypeKind.Array });

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("is of any kind in [List, Array]"));
        }
    }
}
