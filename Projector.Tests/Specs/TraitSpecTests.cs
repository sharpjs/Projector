namespace Projector.Specs
{
    using System;
    using System.Reflection;
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
        public void CreateInstance_ConstructorThrows_Unlikely()
        {
            var e = Assert.Throws<TraitSpecException>
            (
                () => TraitSpec.CreateInstance(typeof(TraitSpec_ConstructorThrows_Unlikely))
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

        internal class TraitSpec_ConstructorThrows : FakeTraitSpec
        {
            public TraitSpec_ConstructorThrows()
            {
                throw new Exception("MessageA");
            }
        }

        internal class TraitSpec_ConstructorThrows_Unlikely : FakeTraitSpec
        {
            public TraitSpec_ConstructorThrows_Unlikely()
            {
                // JS: I think the framework will always throw this type of
                // exception with a non-null inner exception.  However, there
                // is no enforcement of that, so we have to handle it. 
                throw new TargetInvocationException("MessageA", null);
            }
        }

        internal class TraitSpec_NoDefaultConstructor : FakeTraitSpec
        {
            public TraitSpec_NoDefaultConstructor(object unused) { }
        }
    }
}
