using System;
using System.Runtime.Serialization;

namespace Ortus.Archives.Wtb
{
    [Serializable]
    internal class ExternalTextureDataNotLoadedException : Exception
    {
        public ExternalTextureDataNotLoadedException()
        {
        }

        public ExternalTextureDataNotLoadedException( string message ) : base( message )
        {
        }

        public ExternalTextureDataNotLoadedException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected ExternalTextureDataNotLoadedException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}