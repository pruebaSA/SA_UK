namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic128Rsa15SecurityAlgorithmSuite : Basic128SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "Basic128Rsa15";

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaV15KeyWrap;
    }
}

