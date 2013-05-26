namespace Projector.Specs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NamesRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void ToStringMethod()
        {
            var restriction = new NamesRestriction(new[] { "A", "B" }, StringComparison.Ordinal);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("name equals any in [A, B] via Ordinal comparison"));
        }
    }
}
