﻿namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class TripleDesSha256Rsa15SecurityAlgorithmSuite : TripleDesRsa15SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "TripleDesSha256Rsa15";

        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaSha256Signature;

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Sha256Digest;

        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.HmacSha256Signature;
    }
}
