namespace Projector.Utility
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void RemoveInterfacePrefix_Null()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => (null as string).RemoveInterfacePrefix()
            );
        }

        [Test]
        public void RemoveInterfacePrefix_Length1()
        {
            var name = "I".RemoveInterfacePrefix();

            Assert.That(name, Is.EqualTo("I"));
        }

        [Test]
        public void RemoveInterfacePrefix_Prefixed()
        {
            var name = "IInterface".RemoveInterfacePrefix();

            Assert.That(name, Is.EqualTo("Interface"));
        }

        [Test]
        public void RemoveInterfacePrefix_NotPrefixed()
        {
            var name = "NotInterface".RemoveInterfacePrefix();

            Assert.That(name, Is.EqualTo("NotInterface"));
        }

        [Test]
        public void RemoveInterfacePrefix_NotPrefixed_StartsWithI()
        {
            var name = "InterfaceNot".RemoveInterfacePrefix();

            Assert.That(name, Is.EqualTo("InterfaceNot"));
        }
    }
}
