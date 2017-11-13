namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal class Basic192SecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public override bool IsAsymmetricKeyLengthSupported(int length) => 
            ((length >= 0x400) && (length <= 0x1000));

        public override bool IsSymmetricKeyLengthSupported(int length) => 
            ((length >= 0xc0) && (length <= 0x100));

        public override string ToString() => 
            "Basic192";

        public override string DefaultAsymmetricKeyWrapAlgorithm =>
            this.DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap;

        public override string DefaultAsymmetricSignatureAlgorithm =>
            this.DefaultAsymmetricSignatureAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.RsaSha1Signature;

        public override string DefaultCanonicalizationAlgorithm =>
            this.DefaultCanonicalizationAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.ExclusiveC14n;

        public override string DefaultDigestAlgorithm =>
            this.DefaultDigestAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Sha1Digest;

        public override string DefaultEncryptionAlgorithm =>
            this.DefaultEncryptionAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Aes192Encryption;

        public override int DefaultEncryptionKeyDerivationLength =>
            0xc0;

        public override int DefaultSignatureKeyDerivationLength =>
            0xc0;

        public override int DefaultSymmetricKeyLength =>
            0xc0;

        public override string DefaultSymmetricKeyWrapAlgorithm =>
            this.DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.Aes192KeyWrap;

        public override string DefaultSymmetricSignatureAlgorithm =>
            this.DefaultSymmetricSignatureAlgorithmDictionaryString.Value;

        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString =>
            XD.SecurityAlgorithmDictionary.HmacSha1Signature;
    }
}

