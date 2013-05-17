namespace Projector.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using NUnit.Framework.Constraints;
    using Projector.Specs;

    public abstract class StandardTraitResolverTests : ProjectionTestsBase
    {
        [TestFixture]
        public class Invariants
        {
            [Test]
            public void Construct()
            {
                new StandardTraitResolver();
            }

            [Test]
            public void Default()
            {
                Assert.That(StandardTraitResolver.Default, Is.Not.Null);
            }

            [Test]
            public void Construct_NullConfiguration()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => new StandardTraitResolver(null as StandardTraitResolverConfiguration)
                );
            }

            [Test]
            public void Construct_NullConfigure()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => new StandardTraitResolver(null as Action<StandardTraitResolverConfiguration>)
                );
            }

            [Test]
            public void Resolve_NullProjectionType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => new StandardTraitResolver().Resolve(null, typeof(object))
                );
            }

            [Test]
            public void Resolve_NullUnderlyingType()
            {
                Assert.Throws<ArgumentNullException>
                (
                    () => new StandardTraitResolver().Resolve(TypeOf<object>(), null)
                );
            }
        }

        //[TestFixture]
        public class Default
        {
            private readonly StandardTraitResolver
                Resolver = StandardTraitResolver.Default;
        }

        [TestFixture]
        public class Empty : ResolutionCase
        {
            [Test]
            public void TypeTraits()
            {
                Assert.That(GetTypeTraits(), Is.Empty);
            }

            [Test]
            public void PropertyTraits()
            {
                Assert.That(GetPropertyATraits(), Is.Empty);
            }
        }

        [TestFixture]
        public class Included_TypeSpec_TypeTraits : ResolutionCase
        {
            public static readonly object
                TraitA = new object();

            public class SpecA : TypeTraits<ITypeA>
            {
                public SpecA() { Apply(TraitA); }
            }

            protected override void Configure(StandardTraitResolverConfiguration configuration)
            {
                configuration.IncludeSpec<SpecA>();
            }

            [Test]
            public void TypeTraits()
            {
                Assert.That(GetTypeTraits(), HasTraits(TraitA));
            }
        }

        public abstract class ResolutionCase
        {
            public interface ITypeA
            {
                string PropertyA { get; set; }
                string PropertyB { get; set; }
            }

            private ITraitResolution resolution;

            protected virtual void Configure(StandardTraitResolverConfiguration configuration) { }

            [SetUp]
            public void SetUp()
            {
                resolution = new StandardTraitResolver(Configure)
                    .Resolve(TypeOf<ITypeA>(), typeof(ITypeA));
            }

            protected List<object> GetTypeTraits()
            {
                var aggregator = new FakeTraitAggregator();
                resolution.ProvideTypeTraits(aggregator);
                return aggregator.Traits;
            }

            protected List<object> GetPropertyATraits()
            {
                return GetPropertyTraits("PropertyA");
            }

            protected List<object> GetPropertyBTraits()
            {
                return GetPropertyTraits("PropertyB");
            }

            private List<object> GetPropertyTraits(string name)
            {
                var projectionProperty = PropertyOf<ITypeA>(name);
                var underlyingProperty = typeof(ITypeA).GetProperty(name);
                var aggregator         = new FakeTraitAggregator();
                resolution.ProvidePropertyTraits(projectionProperty, underlyingProperty, aggregator);
                return aggregator.Traits;
            }

            protected static Constraint HasTraits(params object[] traits)
            {
                return Is.EqualTo(traits);
            }
        }
    }
}
