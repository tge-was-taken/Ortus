using System;
using System.Runtime.Serialization;

namespace Ortus
{
    [Serializable]
    public class InvalidFileFormatException : Exception
    {
        public InvalidFileFormatException( string message ) : base( message )
        {
        }

        public InvalidFileFormatException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected InvalidFileFormatException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}