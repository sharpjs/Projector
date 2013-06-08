namespace Projector.ObjectModel.StandardTraitResolverConfigurationTests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Projector.Specs;

    [TestFixture]
    public class Invariants : TestCase
    {
        [Test]
        public void IncludeAssembly_NullAssembly()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Configuration.IncludeAssembly(null)
            )
            .ForParameter("assembly");
        }

        [Test]
        public void IncludeAssembly_NullType()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Configuration.IncludeAssemblyOf(null)
            )
            .ForParameter("type");
        }

        [Test]
        public void IncludeSpec_NullSpec()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Configuration.IncludeSpec(null)
            )
            .ForParameter("spec");
        }
    }

    [TestFixture]
    public class Empty : TestCase
    {
        [Test]
        public void IncludedAssemblies()
        {
            Assert.That(Configured.IncludedAssemblies, Is.Null);
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Configured.IncludedSpecs, Is.Null);
        }

        [Test]
        public void GetIncludedAssemblies()
        {
            Assert.That(StandardTraitResolverConfiguration.GetIncludedAssemblies(Configured), Is.Null);
        }

        [Test]
        public void GetIncludedSpecs()
        {
            Assert.That(StandardTraitResolverConfiguration.GetIncludedSpecs(Configured), Is.Null);
        }
    }

    public abstract class WithAssemblies : TestCase
    {
        private readonly Assembly[] Assemblies =
        {
            AssemblyA.Assembly,
            AssemblyB.Assembly
        };

        [Test]
        public void IncludedAssemblies()
        {
            Assert.That(Configured.IncludedAssemblies, Is.EqualTo(Assemblies));
        }

        [Test]
        public void GetAssembliesInternal()
        {
            Assert.That(StandardTraitResolverConfiguration.GetIncludedAssemblies(Configured), Is.EqualTo(Assemblies));
        }
    }

    [TestFixture]
    public class AfterIncludeAssemby : WithAssemblies
    {
        public override void SetUp()
        {
            base.SetUp();
            Configuration
                .IncludeAssembly(AssemblyA.Assembly)
                .IncludeAssembly(AssemblyB.Assembly);
        }
    }

    [TestFixture]
    public class AfterIncludeAssemblyOf_Type : WithAssemblies
    {
        public override void SetUp()
        {
            base.SetUp();
            Configuration
                .IncludeAssemblyOf(typeof(AssemblyA))
                .IncludeAssemblyOf(typeof(AssemblyB));
        }
    }

    [TestFixture]
    public class AfterIncludeAssemblyOf_Generic : WithAssemblies
    {
        public override void SetUp()
        {
            base.SetUp();
            Configuration
                .IncludeAssemblyOf<AssemblyA>()
                .IncludeAssemblyOf<AssemblyB>();
        }
    }

    [TestFixture]
    public class AfterIncludeSpec_Spec : TestCase
    {
        private readonly TraitSpec[] Specs =
        {
            new FakeTraitSpecA(),
            new FakeTraitSpecB()
        };

        public override void SetUp()
        {
            base.SetUp();
            Configuration
                .IncludeSpec(Specs[0])
                .IncludeSpec(Specs[1]);
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Configured.IncludedSpecs, Is.EqualTo(Specs));
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(StandardTraitResolverConfiguration.GetIncludedSpecs(Configured), Is.EqualTo(Specs));
        }
    }

    [TestFixture]
    public class AfterIncludeSpec_Generic : TestCase
    {
        public override void SetUp()
        {
            base.SetUp();
            Configuration
                .IncludeSpec<FakeTraitSpecA>()
                .IncludeSpec<FakeTraitSpecB>();
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Configured.IncludedSpecs, Has.Count.EqualTo(2)
                & Has.Some.TypeOf<FakeTraitSpecA>()
                & Has.Some.TypeOf<FakeTraitSpecB>());
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(StandardTraitResolverConfiguration.GetIncludedSpecs(Configured), Has.Length.EqualTo(2)
                & Has.Some.TypeOf<FakeTraitSpecA>()
                & Has.Some.TypeOf<FakeTraitSpecB>());
        }
    }

    public abstract class TestCase
    {
        protected StandardTraitResolverConfiguration Configuration { get; private set; }

        protected IStandardTraitResolverConfiguration Configured
        {
            get { return Configuration as IStandardTraitResolverConfiguration; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            Configuration = new StandardTraitResolverConfiguration();
        }
    }
}
