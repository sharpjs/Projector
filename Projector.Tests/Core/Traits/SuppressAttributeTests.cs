namespace Projector
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class SuppressAttributeTests
    {
        [Test]
        public void Construct_NullType()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new SuppressAttribute(null)
            );
        }
    }
}
