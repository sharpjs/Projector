namespace Projector
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ProjectionException : Exception
    {
        public ProjectionException()
            : base() { }

        public ProjectionException(string message)
            : base(message) { }

        public ProjectionException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ProjectionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
