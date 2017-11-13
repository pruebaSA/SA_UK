namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal class RSAPKCS1SHA384SignatureDescription : RSAPKCS1SignatureDescription
    {
        public RSAPKCS1SHA384SignatureDescription() : base("SHA384")
        {
        }

        public sealed override HashAlgorithm CreateDigest() => 
            ((HashAlgorithm) CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmldsig-more#sha384"));
    }
}

