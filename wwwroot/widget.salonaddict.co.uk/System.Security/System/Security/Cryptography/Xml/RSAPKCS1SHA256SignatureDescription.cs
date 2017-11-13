namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal class RSAPKCS1SHA256SignatureDescription : RSAPKCS1SignatureDescription
    {
        public RSAPKCS1SHA256SignatureDescription() : base("SHA256")
        {
        }

        public sealed override HashAlgorithm CreateDigest() => 
            ((HashAlgorithm) CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmlenc#sha256"));
    }
}

