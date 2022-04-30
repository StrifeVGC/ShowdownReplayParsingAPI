using System.Runtime.Serialization;

namespace ShowdownReplayParser.Application.Models.Exceptions
{
    [Serializable]
    public class UtilsException : Exception
    {
        public UtilsException()
        {
        }

        public UtilsException(string message)
            : base(message)
        {
        }

        public UtilsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        protected UtilsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
