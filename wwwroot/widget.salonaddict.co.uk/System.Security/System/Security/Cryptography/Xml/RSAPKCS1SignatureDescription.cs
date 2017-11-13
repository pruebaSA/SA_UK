namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal abstract class RSAPKCS1SignatureDescription : SignatureDescription
    {
        public RSAPKCS1SignatureDescription(string hashAlgorithmName)
        {
            base.KeyAlgorithm = "System.Security.Cryptography.RSA";
            base.DigestAlgorithm = hashAlgorithmName;
            base.FormatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureFormatter";
            base.DeformatterAlgorithm = "System.Security.Cryptography.RSAPKCS1SignatureDeformatter";
        }

        public sealed override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            AsymmetricSignatureDeformatter deformatter = new RSAPKCS1SHA2Deformatter();
            deformatter.SetKey(key);
            deformatter.SetHashAlgorithm(base.DigestAlgorithm);
            return deformatter;
        }

        public abstract override HashAlgorithm CreateDigest();
        public sealed override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            AsymmetricSignatureFormatter formatter = new RSAPKCS1SHA2Formatter();
            formatter.SetKey(key);
            formatter.SetHashAlgorithm(base.DigestAlgorithm);
            return formatter;
        }
    }
}

