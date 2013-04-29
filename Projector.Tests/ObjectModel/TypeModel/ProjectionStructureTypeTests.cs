namespace Projector.ObjectModel
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectionStructureTypeTests : ProjectionTestsBase
    {
        private interface IBaseA { }
        private interface IBaseB { }
        private interface IStructureType : IBaseA, IBaseB { }

        private readonly ProjectionType Type = TypeOf<IStructureType>();

        [Test]
        public void Metatype()
        {
            Assert.That(Type, Is.InstanceOf<ProjectionStructureType>()); ;
        }

        [Test]
        public void Kind()
        {
            Assert.That(Type.Kind, Is.EqualTo(TypeKind.Structure));
        }

        [Test]
        public void BaseStructureTypes()
        {
            Assert.That(Type.BaseStructureTypes, Is.EqualTo(new[] { TypeOf<IBaseA>(), TypeOf<IBaseB>() }));
            // See ProjectionTypeCollectionTests
        }

        [Test]
        public void IsVirtualizable()
        {
            Assert.That(Type.IsVirtualizable, Is.True);
        }

        [Test]
        public void CollectionKeyType()
        {
            Assert.That(Type.CollectionKeyType, Is.Null);
        }

        [Test]
        public void CollectionItemType()
        {
            Assert.That(Type.CollectionItemType, Is.Null);
        }
    }
}
