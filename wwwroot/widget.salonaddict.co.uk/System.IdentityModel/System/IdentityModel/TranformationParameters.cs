namespace System.IdentityModel
{
    using System;
    using System.Security.Cryptography;
    using System.Xml;

    internal class TranformationParameters
    {
        public void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
        {
            reader.MoveToContent();
            reader.MoveToStartElement("TransformationParameters", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            string prefix = reader.Prefix;
            bool isEmptyElement = reader.IsEmptyElement;
            reader.ReadStartElement();
            if (reader.IsStartElement(dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod, dictionaryManager.XmlSignatureDictionary.Namespace))
            {
                string attribute = reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, null);
                bool flag2 = reader.IsEmptyElement;
                reader.ReadStartElement();
                if (attribute == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(System.IdentityModel.SR.GetString("RequiredAttributeMissing", new object[] { dictionaryManager.XmlSignatureDictionary.Algorithm, dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod })));
                }
                if (attribute != this.CanonicalizationAlgorithm)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(System.IdentityModel.SR.GetString("AlgorithmMismatchForTransform")));
                }
                if (!flag2)
                {
                    reader.MoveToContent();
                    reader.ReadEndElement();
                }
            }
            if (!isEmptyElement)
            {
                reader.MoveToContent();
                reader.ReadEndElement();
            }
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            writer.WriteStartElement("o", "TransformationParameters", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            writer.WriteStartElement(dictionaryManager.XmlSignatureDictionary.Prefix.Value, dictionaryManager.XmlSignatureDictionary.CanonicalizationMethod, dictionaryManager.XmlSignatureDictionary.Namespace);
            writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, null);
            writer.WriteString(dictionaryManager.SecurityAlgorithmDictionary.ExclusiveC14n);
            writer.WriteEndAttribute();
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public string CanonicalizationAlgorithm =>
            XD.SecurityAlgorithmDictionary.ExclusiveC14n.Value;
    }
}

