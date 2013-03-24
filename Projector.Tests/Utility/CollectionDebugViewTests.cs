namespace Projector.Utility
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class CollectionDebugViewTests
    {
        [Test]
        public void Construct_NullCollection()
        {
            Assert.Throws<ArgumentNullException>
            (
                () => new CollectionDebugView<string>(null)
            );
        }

        [Test]
        public void Items()
        {
            var collection = new[] { "a", "b" };
            var debugView  = new CollectionDebugView<string>(collection);

            Assert.That(debugView.Items, Is.EqualTo(collection));
        }
    }
}
