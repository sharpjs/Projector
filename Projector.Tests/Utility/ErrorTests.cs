namespace Projector.Utility
{
    using NUnit.Framework;

    [TestFixture]
    public class ErrorTests
    {
        [Test]
        public void InternalError()
        {
            // For coverage's sake, this method must be tested directly.
            // There should be no way to throw this exception from the public API.

            var exception = Error.InternalError("achtung");

            Assert.That(exception, Is.Not.Null
                & Has.Message.StringContaining("achtung").IgnoreCase);
        }
    }
}
