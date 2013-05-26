namespace Projector.Specs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NameEqualsRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void Construct_NullName()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new NameRestriction(null, StringComparison.Ordinal)
            )
            .ForParameter("name");
        }

        [Test]
        public void ToStringMethod()
        {
            var restriction = new NameRestriction("SomeName", StringComparison.Ordinal);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("name equals 'SomeName' via Ordinal comparison"));
        }
    }
}
