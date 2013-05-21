namespace Projector.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class TypeCutTests : ProjectionTestsBase
    {
        private TypeCut TypeCut { get; set; }

        [SetUp]
        public void SetUp()
        {
            TypeCut = new TypeCut();
        }

        [Test]
        public void Initially()
        {
            Assert.That(TypeCut.AppliesTo(TypeOf<IAny>()), Is.True);
        }

        [Test]
        public void OfKind_Single_Applies_True()
        {
            TypeCut.OfKind(TypeKind.Array);

            Assert.That(TypeCut.AppliesTo(TypeOf<string[]>()), Is.True);
        }

        [Test]
        public void OfKind_Single_AppliesTo_False()
        {
            TypeCut.OfKind(TypeKind.Array);

            Assert.That(TypeCut.AppliesTo(TypeOf<string  >()), Is.False);
        }

        [Test]
        public void OfKind_Array_Applies_True()
        {
            TypeCut.OfKind(TypeKind.Array, TypeKind.List);

            Assert.That(TypeCut.AppliesTo(TypeOf<string[]     >()), Is.True);
            Assert.That(TypeCut.AppliesTo(TypeOf<IList<string>>()), Is.True);
        }

        [Test]
        public void OfKind_Array_AppliesTo_False()
        {
            TypeCut.OfKind(TypeKind.Array, TypeKind.List);

            Assert.That(TypeCut.AppliesTo(TypeOf<string>()), Is.False);
        }
    }
}
