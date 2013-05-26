namespace Projector.Specs
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class NamesRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void OnNullTypeCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as ITypeCut).Named("A", "B")
            )
            .ForParameter("cut");
        }

        [Test]
        public void OnNullPropertyCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as IPropertyCut).Named("A", "B")
            )
            .ForParameter("cut");
        }

        [Test]
        public void ToStringMethod()
        {
            var restriction = new NamesRestriction(new[] { "A", "B" }, StringComparison.Ordinal);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("name equals any in [A, B] via Ordinal comparison"));
        }
    }
}
