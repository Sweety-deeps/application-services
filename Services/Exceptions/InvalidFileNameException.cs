using System.Runtime.Serialization;

namespace Services.Exceptions
{
    [Serializable]
    internal class InvalidFileNameException : Exception
    {
        public InvalidFileNameException()
        {
        }

        public InvalidFileNameException(string? message) : base(message)
        {
        }

        public InvalidFileNameException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidFileNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}