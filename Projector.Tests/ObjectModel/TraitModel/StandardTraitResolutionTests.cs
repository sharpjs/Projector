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
    public class WithIncludedTraitSpec : TestCase<Fakes.WithNoTraitSpecs.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeSpec<Fakes.TypeATraits>()
                .IncludeSpec<Fakes.SharedTraits>());
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.TypeATraits.TypeA,
                AssemblyA.Fakes.SharedTraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.TypeATraits.PropertyA,
                AssemblyA.Fakes.SharedTraits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithDetectedSharedTraitSpec_InContainingAssembly : TestCase<Fakes.WithSharedTraitSpec.ITypeA>
    {
        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithDetectedTypeTraitSpec_InContainingAssembly : TestCase<Fakes.WithTypeTraitSpec.ITypeA>
    {
        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithDetectedSharedTraitSpec_InIncludedAssembly : TestCase<Fakes.WithSharedTraitSpec.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(AssemblyB.Assembly));
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyB.Fakes.WithSharedTraitSpec.SharedTraits.TypeA,
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.TypeA
                // Containing assembly is always searched; searched last if not present in includes
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyB.Fakes.WithSharedTraitSpec.SharedTraits.PropertyA,
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.PropertyA
                // Containing assembly is always searched; searched last if not present in includes
            ));
        }
    }

    [TestFixture]
    public class WithDetectedTypeTraitSpec_InIncludedAssembly : TestCase<Fakes.WithTypeTraitSpec.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(AssemblyB.Assembly));
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyB.Fakes.WithTypeTraitSpec.TypeATraits.TypeA,
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.TypeA
                // Containing assembly is always searched; searched last if not present in includes
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyB.Fakes.WithTypeTraitSpec.TypeATraits.PropertyA,
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.PropertyA
                // Containing assembly is always searched; searched last if not present in includes
            ));
        }
    }

    [TestFixture]
    public class WithDetectedSharedTraitSpec_InIncludedContainingAssembly : TestCase<Fakes.WithSharedTraitSpec.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(AssemblyA.Assembly)
                .IncludeAssembly(AssemblyB.Assembly));
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.TypeA,
                AssemblyB.Fakes.WithSharedTraitSpec.SharedTraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.WithSharedTraitSpec.SharedTraits.PropertyA,
                AssemblyB.Fakes.WithSharedTraitSpec.SharedTraits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithDetectedTypeTraitSpec_InIncludedContainingAssembly : TestCase<Fakes.WithTypeTraitSpec.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(AssemblyA.Assembly)
                .IncludeAssembly(AssemblyB.Assembly));
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.TypeA,
                AssemblyB.Fakes.WithTypeTraitSpec.TypeATraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.WithTypeTraitSpec.TypeATraits.PropertyA,
                AssemblyB.Fakes.WithTypeTraitSpec.TypeATraits.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithConditionalTraitSpec : TestCase<Fakes.WithNoTraitSpecs.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeSpec<Fakes.ConditionalTraits>());
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                AssemblyA.Fakes.ConditionalTraits.TypeA.Type
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                AssemblyA.Fakes.ConditionalTraits.Global.PropertyA,
                AssemblyA.Fakes.ConditionalTraits.TypeA.PropertyA
            ));
        }
    }

    [TestFixture]
    public class WithManyTraitSpecs : TestCase<Fakes.WithManyTraitSpecs.ITypeA>
    {
        internal override StandardTraitResolver CreateResolver()
        {
            return new StandardTraitResolver(c => c
                .IncludeAssembly(AssemblyB.Assembly)
                .IncludeSpec<Fakes.WithManyTraitSpecs.IncludedTraits>());
        }

        [Test]
        public void ProvideTypeTraits()
        {
            Assert.That(TraitsOfType, IsSequence(
                // Attributes
                new Fakes.WithManyTraitSpecs.FakeAttribute() { Tag = "TypeA" },
                // Included specs
                AssemblyA.Fakes.WithManyTraitSpecs.IncludedTraits.TypeA,
                // Shared specs, in assembly order, then general to specific order
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.Types.Type, // certain  types
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.Type, // specific type
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.Types.Type, // certain  types
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.Type, // specific type
                // Type specs, in assembly order
                AssemblyB.Fakes.WithManyTraitSpecs.TypeATraits.TypeA,
                AssemblyA.Fakes.WithManyTraitSpecs.TypeATraits.TypeA
            ));
        }

        [Test]
        public void ProvidePropertyTraits()
        {
            Assert.That(TraitsOfPropertyA, IsSequence(
                // Attributes
                new Fakes.WithManyTraitSpecs.FakeAttribute() { Tag = "PropertyA" },
                // Included specs, in general to specific order
                AssemblyA.Fakes.WithManyTraitSpecs.IncludedTraits.Properties,     // all      props of specific type
                AssemblyA.Fakes.WithManyTraitSpecs.IncludedTraits.PropertyA,      // specific prop  of specific type
                // Shared specs, in assembly order, then general to specific order
                // ... Assembly B
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.Properties,       // all      props
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.Types.Properties, // all      props of certain  types
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.Types.PropertyA,  // specific prop  of certain  types
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.Properties, // all      props of specific type
                AssemblyB.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.PropertyA,  // specific prop  of specific type
                // ... Assembly A
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.Properties,       // all      props
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.Types.Properties, // all      props of certain  types
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.Types.PropertyA,  // specific prop  of certain  types
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.Properties, // all      props of specific type
                AssemblyA.Fakes.WithManyTraitSpecs.SharedTraits.TypeA.PropertyA,  // specific prop  of specific type
                // Type specs, in assembly order, then general to specific order
                // ... Assembly B
                AssemblyB.Fakes.WithManyTraitSpecs.TypeATraits.Properties,        // all      props of specific type
                AssemblyB.Fakes.WithManyTraitSpecs.TypeATraits.PropertyA,         // specific prop  of specific type
                // ... Assembly A
                AssemblyA.Fakes.WithManyTraitSpecs.TypeATraits.Properties,        // all      props of specific type
                AssemblyA.Fakes.WithManyTraitSpecs.TypeATraits.PropertyA          // specific prop  of specific type
            ));
        }
    }
}
