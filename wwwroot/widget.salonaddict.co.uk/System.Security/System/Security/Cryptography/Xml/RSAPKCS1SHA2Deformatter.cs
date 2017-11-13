namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal class RSAPKCS1SHA2Deformatter : AsymmetricSignatureDeformatter
    {
        private string _hashAlgorithm;
        private RSA _key;

        public override void SetHashAlgorithm(string strName)
        {
            this._hashAlgorithm = strName;
        }

        public override void SetKey(AsymmetricAlgorithm key)
        {
            this._key = (RSA) key;
        }

        public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
        {
            RSACryptoServiceProvider provider = this._key as RSACryptoServiceProvider;
            if ((provider != null) && (provider.CspKeyContainerInfo.ProviderType != 0x18))
            {
                RSAParameters parameters = this._key.ExportParameters(false);
                using (RSACryptoServiceProvider provider2 = new RSACryptoServiceProvider())
                {
                    provider2.ImportParameters(parameters);
                    return provider2.VerifyHash(rgbHash, this._hashAlgorithm, rgbSignature);
                }
            }
            AsymmetricSignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(this._key);
            deformatter.SetHashAlgorithm(this._hashAlgorithm);
            return deformatter.VerifySignature(rgbHash, rgbSignature);
        }
    }
}

