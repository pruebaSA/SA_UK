namespace System.IdentityModel.Tokens
{
    using System;
    using System.IdentityModel;
    using System.Security.Cryptography;

    public class InMemorySymmetricSecurityKey : SymmetricSecurityKey
    {
        private int keySize;
        private byte[] symmetricKey;

        public InMemorySymmetricSecurityKey(byte[] symmetricKey) : this(symmetricKey, true)
        {
        }

        public InMemorySymmetricSecurityKey(byte[] symmetricKey, bool cloneBuffer)
        {
            if (symmetricKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("symmetricKey"));
            }
            if (symmetricKey.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.IdentityModel.SR.GetString("SymmetricKeyLengthTooShort", new object[] { symmetricKey.Length })));
            }
            this.keySize = symmetricKey.Length * 8;
            if (cloneBuffer)
            {
                this.symmetricKey = new byte[symmetricKey.Length];
                Buffer.BlockCopy(symmetricKey, 0, this.symmetricKey, 0, symmetricKey.Length);
            }
            else
            {
                this.symmetricKey = symmetricKey;
            }
        }

        public override byte[] DecryptKey(string algorithm, byte[] keyData) => 
            CryptoHelper.UnwrapKey(this.symmetricKey, keyData, algorithm);

        public override byte[] EncryptKey(string algorithm, byte[] keyData) => 
            CryptoHelper.WrapKey(this.symmetricKey, keyData, algorithm);

        public override byte[] GenerateDerivedKey(string algorithm, byte[] label, byte[] nonce, int derivedKeyLength, int offset) => 
            CryptoHelper.GenerateDerivedKey(this.symmetricKey, algorithm, label, nonce, derivedKeyLength, offset);

        public override ICryptoTransform GetDecryptionTransform(string algorithm, byte[] iv) => 
            CryptoHelper.CreateDecryptor(this.symmetricKey, iv, algorithm);

        public override ICryptoTransform GetEncryptionTransform(string algorithm, byte[] iv) => 
            CryptoHelper.CreateEncryptor(this.symmetricKey, iv, algorithm);

        public override int GetIVSize(string algorithm) => 
            CryptoHelper.GetIVSize(algorithm);

        public override KeyedHashAlgorithm GetKeyedHashAlgorithm(string algorithm) => 
            CryptoHelper.CreateKeyedHashAlgorithm(this.symmetricKey, algorithm);

        public override SymmetricAlgorithm GetSymmetricAlgorithm(string algorithm) => 
            CryptoHelper.GetSymmetricAlgorithm(this.symmetricKey, algorithm);

        public override byte[] GetSymmetricKey()
        {
            byte[] dst = new byte[this.symmetricKey.Length];
            Buffer.BlockCopy(this.symmetricKey, 0, dst, 0, this.symmetricKey.Length);
            return dst;
        }

        public override bool IsAsymmetricAlgorithm(string algorithm) => 
            CryptoHelper.IsAsymmetricAlgorithm(algorithm);

        public override bool IsSupportedAlgorithm(string algorithm) => 
            CryptoHelper.IsSymmetricAlgorithm(algorithm, this.KeySize);

        public override bool IsSymmetricAlgorithm(string algorithm) => 
            CryptoHelper.IsSymmetricAlgorithm(algorithm);

        public override int KeySize =>
            this.keySize;
    }
}

