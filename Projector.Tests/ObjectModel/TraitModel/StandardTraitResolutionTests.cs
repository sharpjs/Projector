namespace Projector.ObjectModel.StandardTraitResolutionTests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;

    public abstract class TestCase<T> : ProjectionTestsBase
    {
        internal StandardTraitResolution Resolution { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Resolution = CreateResolver()
                .Resolve(TypeOf<T>(), typeof(T))
                as StandardTraitResolution;
        }

        internal virtual StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver();
        }

        protected List<object> TraitsOfType
        {
            get { return CollectTypeTraits(); }
        }

        protected List<object> TraitsOfPropertyA
        {
            get { return CollectPropertyTraits("PropertyA"); }
        }

        protected List<object> TraitsOfPropertyB
        {
            get { return CollectPropertyTraits("PropertyB"); }
        }

        private List<object> CollectTypeTraits()
        {
            var aggregator = new FakeTraitAggregator();
            Resolution.ProvideTypeTraits(aggregator);
            return aggregator.Traits;
        }

        private List<object> CollectPropertyTraits(string name)
        {
            var projectionProperty = PropertyOf<T>(name);
            var underlyingProperty = typeof(T).GetProperty(name);
            var aggregator         = new FakeTraitAggregator();
            Resolution.ProvidePropertyTraits(projectionProperty, underlyingProperty, aggregator);
            return aggregator.Traits;
        }
    }

    [TestFixture]
    public class Invariants : TestCase<IAny>
    {
        private readonly ProjectionProperty
            AnyProjectionProperty = PropertyOf<IAny>("Any");

        private readonly PropertyInfo
            AnyUnderlyingProperty = typeof(IAny).GetProperty("Any");

        [Test]
        public void ProjectionType()
        {
            Assert.That(Resolution.ProjectionType, Is.SameAs(TypeOf<IAny>()));
        }

        [Test]
        public void UnderlyingType()
        {
            Assert.That(Resolution.UnderlyingType, Is.SameAs(typeof(IAny)));
        }

        [Test]
        public void ProvidePropertyTraits_NullProjectionProperty()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Resolution.ProvidePropertyTraits(null, AnyUnderlyingProperty, new FakeTraitAggregator())
            );
        }

        [Test]
        public void ProvidePropertyTraits_NullUnderlyingProperty()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Resolution.ProvidePropertyTraits(AnyProjectionProperty, null, new FakeTraitAggregator())
            );
        }

        [Test]
        public void ProvidePropertyTraits_NullAggregator()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Resolution.ProvidePropertyTraits(AnyProjectionProperty, AnyUnderlyingProperty, null)
            );
        }
    }

    [TestFixture]
    public class WithNoTraitSpecs : TestCase<Fakes.WithNoTraitSpecs.ITypeA>
    {
        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, Is.Empty);
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, Is.Empty);
        }
    }

    [TestFixture]
    public class WithDetectedSharedTraitSpec : TestCase<Fakes.WithSharedTraitSpec.ITypeA>
    {
        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                Fakes.WithSharedTraitSpec.Traits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                Fakes.WithSharedTraitSpec.Traits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithDetectedTypeTraitSpec : TestCase<Fakes.WithTypeTraitSpec.ITypeA>
    {
        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                Fakes.WithTypeTraitSpec.Traits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                Fakes.WithTypeTraitSpec.Traits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithIncludedTraitSpec : TestCase<Fakes.WithNoTraitSpecs.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeSpec<Fakes.OtherTraits>());
        }
    }
}
