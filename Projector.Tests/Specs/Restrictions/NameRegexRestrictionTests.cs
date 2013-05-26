namespace Projector.Specs
{
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    [TestFixture]
    public class NameRegexRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void ToStringMethod()
        {
            var restriction = new NameRegexRestriction("Pattern$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("name matches regex 'Pattern$' with options {IgnoreCase, CultureInvariant}"));
        }
    }
}
