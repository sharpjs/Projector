namespace Projector
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class OverrideAttributeTests
    {
        [Test]
        public void Construct_NullSourceType()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new OverrideAttribute(null)
            );
        }
    }
}
