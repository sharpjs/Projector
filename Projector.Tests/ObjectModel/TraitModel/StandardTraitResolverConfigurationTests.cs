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
            );
        }

        [Test]
        public void IncludeSpec_NullSpec()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => Configuration.IncludeSpec(null)
            );
        }
    }

    [TestFixture]
    public class Empty : TestCase
    {
        [Test]
        public void IncludedAssemblies()
        {
            Assert.That(Configuration.IncludedAssemblies, Is.Empty);
        }

        [Test]
        public void IncludedSpecs()
        {
            Assert.That(Configuration.IncludedSpecs, Is.Empty);
        }

        [Test]
        public void GetAssembliesInternal()
        {
            Assert.That(Configuration.GetAssembliesInternal(), Is.Null);
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(Configuration.GetSpecsInternal(), Is.Null);
        }
    }

    [TestFixture]
    public class AfterGetCollections : TestCase
    {
        public override void SetUp()
        {
            base.SetUp();
            object _;
            _ = Configuration.IncludedAssemblies;
            _ = Configuration.IncludedSpecs;
        }

        [Test]
        public void GetAssembliesInternal()
        {
            Assert.That(Configuration.GetAssembliesInternal(), Is.Null);
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(Configuration.GetSpecsInternal(), Is.Null);
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
            Assert.That(Configuration.IncludedAssemblies, Is.EqualTo(Assemblies));
        }

        [Test]
        public void GetAssembliesInternal()
        {
            Assert.That(Configuration.GetAssembliesInternal(), Is.EqualTo(Assemblies));
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
            Assert.That(Configuration.IncludedSpecs, Is.EqualTo(Specs));
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(Configuration.GetSpecsInternal(), Is.EqualTo(Specs));
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
            Assert.That(Configuration.IncludedSpecs, Has.Count.EqualTo(2)
                & Has.Some.TypeOf<FakeTraitSpecA>()
                & Has.Some.TypeOf<FakeTraitSpecB>());
        }

        [Test]
        public void GetSpecsInternal()
        {
            Assert.That(Configuration.GetSpecsInternal(), Has.Length.EqualTo(2)
                & Has.Some.TypeOf<FakeTraitSpecA>()
                & Has.Some.TypeOf<FakeTraitSpecB>());
        }
    }

    public abstract class TestCase
    {
        protected StandardTraitResolverConfiguration Configuration { get; private set; }

        [SetUp]
        public virtual void SetUp()
        {
            Configuration = new StandardTraitResolverConfiguration();
        }
    }
}
