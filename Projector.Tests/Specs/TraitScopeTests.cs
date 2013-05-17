namespace Projector.Specs
{
    using System;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TraitScopeTests
    {
        private TraitScope Scope;

        [SetUp]
        public void SetUp()
        {
            Scope = new TraitScope();
        }

        [Test]
        public void Apply_NullTrait()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Scope.Apply(null as object)
            );
        }

        [Test]
        public void Apply_NullFactory()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Scope.Apply(null as Func<object>)
            );
        }

        [Test]
        public void ProvideTraits_Traits()
        {
            var traits     = new[] { new object(), new object() };
            var aggregator = new FakeTraitAggregator();

            Scope.Apply(traits[0]).Apply(traits[1]);
            Scope.ProvideTraits(aggregator);

            Assert.That(aggregator.Traits, Is.EqualTo(traits));
        }

        [Test]
        public void ProvideTraits_Factories()
        {
            var traits     = new      object [] { new object(),    new object()    };
            var factories  = new Func<object>[] { () => traits[0], () => traits[1] };
            var aggregator = new FakeTraitAggregator();

            Scope.Apply(factories[0]).Apply(factories[1]);
            Scope.ProvideTraits(aggregator);

            Assert.That(aggregator.Traits, Is.EqualTo(traits));
        }

        [Test]
        public void ProvideTraits_FactoryReturnsNull()
        {
            Func<object> factory = () => null;
            var aggregator       = new FakeTraitAggregator();

            Scope.Apply(factory);

            Assert.Throws<TraitSpecException>
            (
                () => Scope.ProvideTraits(aggregator)
            );

            Assert.That(aggregator.Traits, Is.Empty);
        }
    }
}
