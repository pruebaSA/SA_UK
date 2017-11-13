namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;

    [Serializable]
    internal class CryptoSignedXmlRecursionException : XmlException
    {
        public CryptoSignedXmlRecursionException()
        {
        }

        public CryptoSignedXmlRecursionException(string message) : base(message)
        {
        }

        protected CryptoSignedXmlRecursionException(SerializationInfo info, StreamingContext context)
        {
        }

        public CryptoSignedXmlRecursionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

