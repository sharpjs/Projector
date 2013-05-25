namespace Projector.Specs
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TypeTraitSpecTests : ProjectionTestsBase
    {
        private FakeTypeATraits Spec { get; set; }

        [SetUp]
        public void SetUp()
        {
            Spec = new FakeTypeATraits();
            Spec.Build();
        }

        [Test]
        public void Build()
        {
            Assert.That(Spec.BuildCount, Is.EqualTo(1));
        }

        [Test]
        public void ProvideScopes_TargetType()
        {
            var aggregator = new FakeTypeScopeAggregator();

            Spec.ProvideScopes(TypeOf<ITypeA>(), typeof(ITypeA), aggregator);

            Assert.That(aggregator.Scopes, Has.Count.EqualTo(1));
        }

        [Test]
        public void ProvideScopes_OtherType()
        {
            var aggregator = new FakeTypeScopeAggregator();

            Spec.ProvideScopes(TypeOf<ITypeB>(), typeof(ITypeB), aggregator);

            Assert.That(aggregator.Scopes, Is.Empty);
        }

        private interface ITypeA { }
        private interface ITypeB { }

        private class FakeTypeATraits : TypeTraitSpec<ITypeA>
        {
            public int BuildCount { get; private set; }

            protected override void Build(ITypeScope<ITypeA> scope)
            {
                BuildCount++;
            }
        }
    }
}
