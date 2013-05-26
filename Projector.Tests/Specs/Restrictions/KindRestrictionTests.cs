namespace Projector.Specs
{
    using System;
    using NUnit.Framework;
    using Projector.ObjectModel;

    [TestFixture]
    public class KindRestrictionTests
    {
        // Other tests in xxxxCutTests

        [Test]
        public void OnNullTypeCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as ITypeCut).OfKind(TypeKind.Array)
            )
            .ForParameter("cut");
        }

        [Test]
        public void OnNullPropertyCut()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as IPropertyCut).OfKind(TypeKind.Array)
            )
            .ForParameter("cut");
        }

        [Test]
        public void ToStringMethod()
        {
            var restriction = new KindRestriction(TypeKind.List);

            var text = restriction.ToString();

            Assert.That(text, Is.EqualTo("is of List kind"));
        }
    }
}
