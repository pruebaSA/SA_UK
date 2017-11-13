namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Cryptography;

    internal class RSAPKCS1SHA2Formatter : AsymmetricSignatureFormatter
    {
        private string _hashAlgorithm;
        private RSA _key;

        public override byte[] CreateSignature(byte[] rgbHash)
        {
            RSACryptoServiceProvider rsaCsp = this._key as RSACryptoServiceProvider;
            if (rsaCsp != null)
            {
                using (RSACryptoServiceProvider provider2 = UpgradeCspIfNeeded(rsaCsp))
                {
                    RSACryptoServiceProvider provider3 = provider2 ?? rsaCsp;
                    return provider3.SignHash(rgbHash, this._hashAlgorithm);
                }
            }
            AsymmetricSignatureFormatter formatter = new RSAPKCS1SignatureFormatter(this._key);
            formatter.SetHashAlgorithm(this._hashAlgorithm);
            return formatter.CreateSignature(rgbHash);
        }

        public override void SetHashAlgorithm(string strName)
        {
            this._hashAlgorithm = strName;
        }

        public override void SetKey(AsymmetricAlgorithm key)
        {
            this._key = (RSA) key;
        }

        private static bool ShouldUpgrade(CspKeyContainerInfo keyContainerInfo)
        {
            switch (keyContainerInfo.ProviderType)
            {
                case 1:
                case 2:
                case 12:
                {
                    string providerName = keyContainerInfo.ProviderName;
                    StringComparison ordinalIgnoreCase = StringComparison.OrdinalIgnoreCase;
                    if (((!providerName.Equals("Microsoft Base Cryptographic Provider v1.0", ordinalIgnoreCase) && !providerName.Equals("Microsoft RSA Signature Cryptographic Provider", ordinalIgnoreCase)) && (!providerName.Equals("Microsoft Enhanced Cryptographic Provider v1.0", ordinalIgnoreCase) && !providerName.Equals("Microsoft Strong Cryptographic Provider", ordinalIgnoreCase))) && (!providerName.Equals("Microsoft Enhanced RSA and AES Cryptographic Provider", ordinalIgnoreCase) && !providerName.Equals("Microsoft Enhanced RSA and AES Cryptographic Provider (Prototype)", ordinalIgnoreCase)))
                    {
                        return false;
                    }
                    return true;
                }
                case 0x18:
                    return false;
            }
            return false;
        }

        private static RSACryptoServiceProvider UpgradeCspIfNeeded(RSACryptoServiceProvider rsaCsp)
        {
            CspKeyContainerInfo cspKeyContainerInfo = rsaCsp.CspKeyContainerInfo;
            if (!ShouldUpgrade(cspKeyContainerInfo))
            {
                return null;
            }
            CspParameters parameters = new CspParameters(0x18) {
                KeyContainerName = cspKeyContainerInfo.KeyContainerName,
                Flags = CspProviderFlags.UseExistingKey
            };
            if (cspKeyContainerInfo.MachineKeyStore)
            {
                parameters.Flags |= CspProviderFlags.UseMachineKeyStore;
            }
            parameters.KeyNumber = (int) cspKeyContainerInfo.KeyNumber;
            try
            {
                return new RSACryptoServiceProvider(parameters);
            }
            catch (CryptographicException)
            {
                return null;
            }
        }
    }
}

