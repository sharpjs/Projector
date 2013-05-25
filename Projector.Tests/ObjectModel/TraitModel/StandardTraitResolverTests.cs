namespace Projector.ObjectModel.StandardTraitResolverTests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Projector.Specs;

    public abstract class TestCase : ProjectionTestsBase
    {
        internal StandardTraitResolver Resolver { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Resolver = CreateResolver();
        }

        internal virtual StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver();
        }

        internal StandardTraitResolution Resolve<T>()
        {
            return Resolver.Resolve(TypeOf<T>(), typeof(T)) as StandardTraitResolution;
        }
    }

    [TestFixture]
    public class Invariants : TestCase
    {
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
                () => Resolver.Resolve(null, typeof(IAny))
            );
        }

        [Test]
        public void Resolve_NullUnderlyingType()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Resolver.Resolve(TypeOf<IAny>(), null)
            );
        }
    }

    public abstract class Default : TestCase
    {
        [Test]
        public void IncludedAssemblies()
        {
            Assert.That(Resolver.IncludedAssemblies, Is.Empty);
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Resolver.IncludedSpecs, Is.Empty);
        }
    }

    [TestFixture]
    public class FromDefaultConstructor : Default
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver();
        }
    }

    [TestFixture]
    public class FromDefaultProperty : Default
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return StandardTraitResolver.Default;
        }

        [Test]
        public void Default()
        {
            Assert.That(StandardTraitResolver.Default, Is.SameAs(Resolver));
        }
    }

    public abstract class Configured : TestCase
    {
        internal Assembly  Assembly  { get; private set; }
        internal TraitSpec TraitSpec { get; private set; }

        [SetUp]
        public override void SetUp()
        {
            Assembly  = AssemblyA.Assembly;
            TraitSpec = new FakeTraitSpec();
            base.SetUp();
        }

        [Test]
        public void IncludedAssemblies()
        {
            Assert.That(Resolver.IncludedAssemblies, IsSequence(Assembly));
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Resolver.IncludedSpecs, IsSequence(TraitSpec));
        }
    }

    [TestFixture]
    public class FromConfigure : Configured
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(Assembly)
                .IncludeSpec(TraitSpec));
        }
    }

    [TestFixture]
    public class FromConfiguration : Configured
    {
        internal override StandardTraitResolver CreateResolver()
        {
            var configuration = new StandardTraitResolverConfiguration()
                .IncludeAssembly(Assembly)
                .IncludeSpec(TraitSpec);
            return new StandardTraitResolver(configuration);
        }
    }
}
