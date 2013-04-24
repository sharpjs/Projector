namespace Projector.Tests.ObjectModel
{
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TypeKindTests
    {
        [Test]
        public void IsCollection_Opaque()
        {
            Assert.That(TypeKind.Opaque.IsCollection(), Is.False);
        }

        [Test]
        public void IsCollection_Projection()
        {
            Assert.That(TypeKind.Structure.IsCollection(), Is.False);
        }

        [Test]
        public void IsCollection_Array()
        {
            Assert.That(TypeKind.Array.IsCollection(), Is.True);
        }

        [Test]
        public void IsCollection_List()
        {
            Assert.That(TypeKind.List.IsCollection(), Is.True);
        }

        [Test]
        public void IsCollection_Set()
        {
            Assert.That(TypeKind.Set.IsCollection(), Is.True);
        }

        [Test]
        public void IsCollection_Dictionary()
        {
            Assert.That(TypeKind.Dictionary.IsCollection(), Is.True);
        }
    }
}
