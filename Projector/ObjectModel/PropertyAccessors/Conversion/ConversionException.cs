namespace Projector.ObjectModel
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ConversionException : FormatException
    {
        public ConversionException()
            : base() { }

        public ConversionException(string message)
            : base(message) { }

        public ConversionException(string message, Exception innerException)
            : base(message, innerException) { }

        protected ConversionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
