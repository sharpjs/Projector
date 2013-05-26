namespace Projector.Specs
{
    using System;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class KindsRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void OnNullTypeCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as ITypeCut).OfKind(TypeKind.Array, TypeKind.List)
            )
            .ForParameter("cut");
        }

        [Test]
        public void OnNullPropertyCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as IPropertyCut).OfKind(TypeKind.Array, TypeKind.List)
            )
            .ForParameter("cut");
        }

        [Test]
        public void ToStringMethod()
        {
            var restriction = new KindsRestriction(new[] { TypeKind.List, TypeKind.Array });

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("is of any kind in [List, Array]"));
        }
    }
}
