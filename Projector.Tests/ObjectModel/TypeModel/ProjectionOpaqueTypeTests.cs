namespace Projector.ObjectModel
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ProjectionOpaqueTypeTests : ProjectionTestsBase
    {
        private sealed class OpaqueType { };

        private readonly ProjectionType Type = TypeOf<OpaqueType>();

        [Test]
        public void Kind()
        {
            Assert.That(Type.Kind, Is.EqualTo(TypeKind.Opaque));
        }

        [Test]
        public void IsVirtualizable()
        {
            Assert.That(Type.IsVirtualizable, Is.False);
        }
    }
}
