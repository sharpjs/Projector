namespace Projector.ObjectModel
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ProjectionTypeTests : ProjectionTestsBase
    {
        private sealed class AnyType { };

        private readonly ProjectionType Type = TypeOf<AnyType>();

        [Test]
        public new void Factory()
        {
            Assert.That(Type.Factory, Is.SameAs(ProjectionTestsBase.Factory));
            // Factory always gets the factory that created the type
        }

        [Test]
        public void UnderlyingType()
        {
            Assert.That(Type.UnderlyingType, Is.EqualTo(typeof(AnyType)));
        }

        [Test]
        public void Implicit_Type_NotNulll()
        {
            Type convertedType = Type;

            Assert.That(convertedType, Is.EqualTo(typeof(AnyType)));
        }

        [Test]
        public void Implicit_Type_Null()
        {
            var  nullType      = null as ProjectionType;
            Type convertedType = nullType;

            Assert.That(convertedType, Is.Null);
        }

        [Test]
        public void Name()
        {
            Assert.That(Type.Name, Is.EqualTo(typeof(AnyType).GetPrettyName(false)));
        }

        [Test]
        public void FullName()
        {
            Assert.That(Type.FullName, Is.EqualTo(typeof(AnyType).GetPrettyName(true)));
        }

        [Test]
        public void ToStringMethod()
        {
            Assert.That(Type.ToString(), Is.EqualTo(typeof(AnyType).GetPrettyName(true)));
        }

        [Test]
        public void Namespace()
        {
            Assert.That(Type.Namespace, Is.EqualTo(typeof(AnyType).Namespace));
        }
    }
}
