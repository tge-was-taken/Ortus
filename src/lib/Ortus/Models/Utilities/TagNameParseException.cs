using System;
using System.Runtime.Serialization;

namespace Ortus.Models.Utilities
{
    [Serializable]
    internal class TagNameParseException : Exception
    {
        public TagNameParseException()
        {
        }

        public TagNameParseException( string message ) : base( message )
        {
        }

        public TagNameParseException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected TagNameParseException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}