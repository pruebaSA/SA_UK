namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic256Rsa15SecurityAlgorithmSuite : Basic256SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "Basic256Rsa15";

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaV15KeyWrap;
    }
}

