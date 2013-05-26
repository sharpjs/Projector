namespace Projector.Specs
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class PredicateRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void ToStringMethod_Default()
        {
            var restriction = new FakePredicateRestriction(null);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("matches predicate"));
        }

        [Test]
        public void ToStringMethod_Explicit()
        {
            var restriction = new FakePredicateRestriction("some message");

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("some message"));
        }

        private class FakePredicateRestriction : PredicateRestriction<ProjectionMetaObject>
        {
            public FakePredicateRestriction(string description)
                : base(o => true, description) { }
        }
    }
}
