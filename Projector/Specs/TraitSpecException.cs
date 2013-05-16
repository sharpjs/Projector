namespace Projector.Specs
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TraitSpecException : ApplicationException
    {
        public TraitSpecException()
            : base() { }

        public TraitSpecException(string message)
            : base(message) { }

        public TraitSpecException(string message, Exception innerException)
            : base(message, innerException) { }

        protected TraitSpecException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
