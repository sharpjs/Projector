namespace Projector
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    [TestFixture]
    public class ProjectionExceptionTests
    {
        [Test]
        public void Construct_Default()
        {
            var exception = new ProjectionException();
        }

        [Test]
        public void Construct_Message()
        {
            var exception = new ProjectionException("Message");

            Assert.That(exception.Message, Is.EqualTo("Message"));
        }

        [Test]
        public void Construct_Message_InnerException()
        {
            var innerException = new Exception();
            var exception = new ProjectionException("Message", innerException);

            Assert.That(exception.Message,        Is.EqualTo("Message"));
            Assert.That(exception.InnerException, Is.SameAs(innerException));
        }

        [Test]
        public void Serialization_RoundTrip()
        {
            var exception = new ProjectionException("MessageA", new Exception("MessageB"));
            var formatter = new BinaryFormatter();

            using (var stream = new MemoryStream(512))
            {
                formatter.Serialize(stream, exception);
                stream.Position = 0;
                exception = (ProjectionException) formatter.Deserialize(stream);
            }

            Assert.That(exception,                        Is.Not.Null);
            Assert.That(exception.Message,                Is.EqualTo("MessageA"));
            Assert.That(exception.InnerException,         Is.Not.Null);
            Assert.That(exception.InnerException.Message, Is.EqualTo("MessageB"));
        }
    }
}
