namespace Projector
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class InheritFromAttributeTests
    {
        [Test]
        public void Construct_NullSourceType()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new InheritFromAttribute(null)
            );
        }
    }
}
