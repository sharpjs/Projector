namespace Projector.ObjectModel
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TraitOptionsTests
    {
        [Test]
        public void GetTraitOptions_Default()
        {
            var source = new object();

            var expected = new TraitOptions
            {
                ValidOn       = AttributeTargets.All,
                AllowMultiple = false,
                Inherited     = true
            };

            AssertTraitOptions(source, expected);
        }

        [Test]
        public void GetTraitOptions_ViaAttributeUsage()
        {
            var source = new FakeAttribute();

            var expected = new TraitOptions
            {
                ValidOn       = AttributeTargets.Property,
                AllowMultiple = true,
                Inherited     = false
            };

            AssertTraitOptions(source, expected);
        }

        [Test]
        public void GetTraitOptions_ViaAttributeUsage_Cached()
        {
            var source = new FakeAttribute();

            Assert.AreSame(source.GetTraitOptions(), source.GetTraitOptions());
        }

        [Test]
        public void GetTraitOptions_ViaInterface()
        {
            var source = new MockAnnotation
            {
                ValidOn       = AttributeTargets.Interface,
                AllowMultiple = true,
                Inherited     = false
            };

            var expected = new TraitOptions
            {
                ValidOn       = AttributeTargets.Interface,
                AllowMultiple = true,
                Inherited     = false
            };

            AssertTraitOptions(source, expected);
        }

        [Test]
        public void GetTraitOptions_ViaInterface_OverridingAttributeUsage()
        {
            var source = new MockAnnotationAttribute
            {
                ValidOn       = AttributeTargets.Interface,
                AllowMultiple = true,
                Inherited     = false
            };

            var expected = new TraitOptions
            {
                ValidOn       = AttributeTargets.Interface,
                AllowMultiple = true,
                Inherited     = false
            };

            AssertTraitOptions(source, expected);
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
        private class FakeAttribute : Attribute { }

        private class TraitOptions : MockAnnotation { }

        private static void AssertTraitOptions(object source, ITraitOptions expected)
        {
            var options = source.GetTraitOptions();

            Assert.That(options.ValidOn,       Is.EqualTo(expected.ValidOn),       "ValidOn");
            Assert.That(options.AllowMultiple, Is.EqualTo(expected.AllowMultiple), "AllowMultiple");
            Assert.That(options.Inherited,     Is.EqualTo(expected.Inherited),     "Inherited");
        }
    }
}
