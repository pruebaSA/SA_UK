namespace System.IdentityModel
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Xml;

    internal class StrTransform : Transform
    {
        private readonly bool includeComments = false;
        private string inclusiveNamespacesPrefixList;
        private string[] inclusivePrefixes;
        private string prefix = "";
        private TranformationParameters transformationParameters = new TranformationParameters();

        private CanonicalizationDriver GetConfiguredDriver(SignatureResourcePool resourcePool)
        {
            CanonicalizationDriver driver = resourcePool.TakeCanonicalizationDriver();
            driver.IncludeComments = this.IncludeComments;
            driver.SetInclusivePrefixes(this.inclusivePrefixes);
            return driver;
        }

        public string[] GetInclusivePrefixes() => 
            this.inclusivePrefixes;

        public override object Process(object input, SignatureResourcePool resourcePool, DictionaryManager dictionaryManager)
        {
            if (input is XmlReader)
            {
                CanonicalizationDriver configuredDriver = this.GetConfiguredDriver(resourcePool);
                configuredDriver.SetInput(input as XmlReader);
                return configuredDriver.GetMemoryStream();
            }
            if (!(input is ISecurityElement))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.IdentityModel.SR.GetString("UnsupportedInputTypeForTransform", new object[] { input.GetType() })));
            }
            MemoryStream stream = new MemoryStream();
            XmlDictionaryWriter writer = resourcePool.TakeUtf8Writer();
            writer.StartCanonicalization(stream, false, null);
            (input as ISecurityElement).WriteTo(writer, dictionaryManager);
            writer.EndCanonicalization();
            stream.Seek(0L, SeekOrigin.Begin);
            return stream;
        }

        public void ProcessAndDigest(object input, SignatureResourcePool resourcePool, HashAlgorithm hash, DictionaryManager dictionaryManger)
        {
            HashStream hashStream = resourcePool.TakeHashStream(hash);
            XmlReader reader = input as XmlReader;
            if (reader != null)
            {
                this.ProcessReaderInput(reader, resourcePool, hashStream);
            }
            else
            {
                if (!(input is ISecurityElement))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.IdentityModel.SR.GetString("UnsupportedInputTypeForTransform", new object[] { input.GetType() })));
                }
                XmlDictionaryWriter writer = resourcePool.TakeUtf8Writer();
                writer.StartCanonicalization(hashStream, this.IncludeComments, this.GetInclusivePrefixes());
                (input as ISecurityElement).WriteTo(writer, dictionaryManger);
                writer.EndCanonicalization();
            }
            hashStream.FlushHash();
        }

        public override byte[] ProcessAndDigest(object input, SignatureResourcePool resourcePool, string digestAlgorithm, DictionaryManager dictionaryManager)
        {
            HashAlgorithm hash = resourcePool.TakeHashAlgorithm(digestAlgorithm);
            this.ProcessAndDigest(input, resourcePool, hash, dictionaryManager);
            return hash.Hash;
        }

        private void ProcessReaderInput(XmlReader reader, SignatureResourcePool resourcePool, HashStream hashStream)
        {
            reader.MoveToContent();
            XmlDictionaryReader reader2 = reader as XmlDictionaryReader;
            if ((reader2 != null) && reader2.CanCanonicalize)
            {
                reader2.StartCanonicalization(hashStream, this.IncludeComments, this.GetInclusivePrefixes());
                reader2.Skip();
                reader2.EndCanonicalization();
            }
            else
            {
                CanonicalizationDriver configuredDriver = this.GetConfiguredDriver(resourcePool);
                configuredDriver.SetInput(reader);
                configuredDriver.WriteTo(hashStream);
            }
        }

        public override void ReadFrom(XmlDictionaryReader reader, DictionaryManager dictionaryManager)
        {
            reader.MoveToStartElement(dictionaryManager.XmlSignatureDictionary.Transform, dictionaryManager.XmlSignatureDictionary.Namespace);
            this.prefix = reader.Prefix;
            bool isEmptyElement = reader.IsEmptyElement;
            if (reader.GetAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, null) != this.Algorithm)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(System.IdentityModel.SR.GetString("AlgorithmMismatchForTransform")));
            }
            reader.MoveToContent();
            reader.Read();
            if (!isEmptyElement)
            {
                if (reader.IsStartElement("TransformationParameters", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"))
                {
                    this.transformationParameters.ReadFrom(reader, dictionaryManager);
                }
                reader.MoveToContent();
                reader.ReadEndElement();
            }
        }

        private static string[] TokenizeInclusivePrefixList(string prefixList)
        {
            if (prefixList == null)
            {
                return null;
            }
            string[] sourceArray = prefixList.Split(null);
            int length = 0;
            for (int i = 0; i < sourceArray.Length; i++)
            {
                string str = sourceArray[i];
                if (str == "#default")
                {
                    sourceArray[length++] = string.Empty;
                }
                else if (str.Length > 0)
                {
                    sourceArray[length++] = str;
                }
            }
            if (length == 0)
            {
                return null;
            }
            if (length == sourceArray.Length)
            {
                return sourceArray;
            }
            string[] destinationArray = new string[length];
            Array.Copy(sourceArray, destinationArray, length);
            return destinationArray;
        }

        public override void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            writer.WriteStartElement(this.prefix, dictionaryManager.XmlSignatureDictionary.Transform, dictionaryManager.XmlSignatureDictionary.Namespace);
            writer.WriteStartAttribute(dictionaryManager.XmlSignatureDictionary.Algorithm, null);
            writer.WriteString(this.Algorithm);
            writer.WriteEndAttribute();
            this.transformationParameters.WriteTo(writer, dictionaryManager);
            writer.WriteEndElement();
        }

        public override string Algorithm =>
            "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform";

        public bool IncludeComments =>
            this.includeComments;

        public string InclusiveNamespacesPrefixList
        {
            get => 
                this.inclusiveNamespacesPrefixList;
            set
            {
                this.inclusiveNamespacesPrefixList = value;
                this.inclusivePrefixes = TokenizeInclusivePrefixList(value);
            }
        }

        public override bool NeedsInclusiveContext =>
            (this.GetInclusivePrefixes() != null);
    }
}

