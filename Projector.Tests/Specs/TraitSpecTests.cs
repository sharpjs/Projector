namespace Projector.Specs
{
    using System;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TraitSpecTests
    {
        [Test]
        public void CreateInstance_Success()
        {
            var spec = TraitSpec.CreateInstance(typeof(FakeTraitSpec));

            Assert.That(spec, Is.TypeOf<FakeTraitSpec>());
        }

        [Test]
        public void CreateInstance_WrongType()
        {
            var e = Assert.Throws<TraitSpecException>
            (
                () => TraitSpec.CreateInstance(typeof(object))
            );
            Assert.That(e.InnerException, Is.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void CreateInstance_ConstructorThrows()
        {
            var e = Assert.Throws<TraitSpecException>
            (
                () => TraitSpec.CreateInstance(typeof(TraitSpec_ConstructorThrows))
            );
            Assert.That(e.InnerException, Is.Not.Null & Has.Message.EqualTo("MessageA"));
        }

        [Test]
        public void CreateInstance_OtherProblem()
        {
            var e = Assert.Throws<TraitSpecException>
            (
                () => TraitSpec.CreateInstance(typeof(TraitSpec_NoDefaultConstructor))
            );
            Assert.That(e.InnerException, Is.Not.Null);
        }

        internal class FakeTraitSpec : TraitSpec
        {
            internal override void ProvideScopes(ProjectionType projectionType, Type underlyingType, ITypeScopeAggregator action)
            {
                // Stub only
            }
        }

        internal class TraitSpec_ConstructorThrows : FakeTraitSpec
        {
            public TraitSpec_ConstructorThrows() { throw new Exception("MessageA"); }
        }

        internal class TraitSpec_NoDefaultConstructor : FakeTraitSpec
        {
            public TraitSpec_NoDefaultConstructor(object unused) { }
        }
    }
}
