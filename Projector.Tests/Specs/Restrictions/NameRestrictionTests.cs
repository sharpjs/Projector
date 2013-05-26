namespace Projector.Specs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NameRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void OnNullTypeCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as ITypeCut).Named("A")
            )
            .ForParameter("cut");
        }

        [Test]
        public void OnNullPropertyCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as IPropertyCut).Named("A")
            )
            .ForParameter("cut");
        }

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
