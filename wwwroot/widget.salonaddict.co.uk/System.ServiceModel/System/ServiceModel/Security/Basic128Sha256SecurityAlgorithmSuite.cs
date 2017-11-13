namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic128Sha256SecurityAlgorithmSuite : Basic128SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "Basic128Sha256";

        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaSha256Signature;

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Sha256Digest;

        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.HmacSha256Signature;
    }
}

