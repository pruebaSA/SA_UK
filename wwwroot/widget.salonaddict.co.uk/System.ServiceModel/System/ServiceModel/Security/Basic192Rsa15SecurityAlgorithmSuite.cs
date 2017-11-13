namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic192Rsa15SecurityAlgorithmSuite : Basic192SecurityAlgorithmSuite
    {
        public override string ToString() => 
            "Basic192Rsa15";

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaV15KeyWrap;
    }
}

