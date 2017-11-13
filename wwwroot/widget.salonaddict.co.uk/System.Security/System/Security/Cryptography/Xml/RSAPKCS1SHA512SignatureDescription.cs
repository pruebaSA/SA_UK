﻿namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal class RSAPKCS1SHA512SignatureDescription : RSAPKCS1SignatureDescription
    {
        public RSAPKCS1SHA512SignatureDescription() : base("SHA512")
        {
        }

        public sealed override HashAlgorithm CreateDigest() => 
            ((HashAlgorithm) CryptoConfig.CreateFromName("http://www.w3.org/2001/04/xmlenc#sha512"));
    }
}
