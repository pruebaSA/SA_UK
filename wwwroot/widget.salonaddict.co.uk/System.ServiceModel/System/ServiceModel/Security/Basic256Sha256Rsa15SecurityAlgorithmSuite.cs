namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic256Sha256Rsa15SecurityAlgorithmSuite : Basic256Rsa15SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "Basic256Sha256Rsa15";

        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaSha256Signature;

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Sha256Digest;

        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.HmacSha256Signature;
    }
}

