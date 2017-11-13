namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class TripleDesRsa15SecurityAlgorithmSuite : TripleDesSecurityAlgorithmSuite
    {
        public override string ToString() => 
            "TripleDesRsa15";

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaV15KeyWrap;
    }
}

