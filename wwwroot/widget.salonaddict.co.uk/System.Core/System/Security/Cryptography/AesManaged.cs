namespace System.Security.Cryptography
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class AesManaged : Aes
    {
        private RijndaelManaged m_rijndael;

        public AesManaged()
        {
            if (CoreCryptoConfig.EnforceFipsAlgorithms)
            {
                throw new InvalidOperationException(System.SR.GetString("Cryptography_NonCompliantFIPSAlgorithm"));
            }
            this.m_rijndael = new RijndaelManaged();
            this.m_rijndael.BlockSize = this.BlockSize;
            this.m_rijndael.KeySize = this.KeySize;
        }

        public override ICryptoTransform CreateDecryptor() => 
            this.m_rijndael.CreateDecryptor();

        public override ICryptoTransform CreateDecryptor(byte[] key, byte[] iv)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!base.ValidKeySize(key.Length * 8))
            {
                throw new ArgumentException(System.SR.GetString("Cryptography_InvalidKeySize"), "key");
            }
            if ((iv != null) && ((iv.Length * 8) != base.BlockSizeValue))
            {
                throw new ArgumentException(System.SR.GetString("Cryptography_InvalidIVSize"), "iv");
            }
            return this.m_rijndael.CreateDecryptor(key, iv);
        }

        public override ICryptoTransform CreateEncryptor() => 
            this.m_rijndael.CreateEncryptor();

        public override ICryptoTransform CreateEncryptor(byte[] key, byte[] iv)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!base.ValidKeySize(key.Length * 8))
            {
                throw new ArgumentException(System.SR.GetString("Cryptography_InvalidKeySize"), "key");
            }
            if ((iv != null) && ((iv.Length * 8) != base.BlockSizeValue))
            {
                throw new ArgumentException(System.SR.GetString("Cryptography_InvalidIVSize"), "iv");
            }
            return this.m_rijndael.CreateEncryptor(key, iv);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                ((IDisposable) this.m_rijndael).Dispose();
            }
        }

        public override void GenerateIV()
        {
            this.m_rijndael.GenerateIV();
        }

        public override void GenerateKey()
        {
            this.m_rijndael.GenerateKey();
        }

        public override int FeedbackSize
        {
            get => 
                this.m_rijndael.FeedbackSize;
            set
            {
                this.m_rijndael.FeedbackSize = value;
            }
        }

        public override byte[] IV
        {
            get => 
                this.m_rijndael.IV;
            set
            {
                this.m_rijndael.IV = value;
            }
        }

        public override byte[] Key
        {
            get => 
                this.m_rijndael.Key;
            set
            {
                this.m_rijndael.Key = value;
            }
        }

        public override int KeySize
        {
            get => 
                this.m_rijndael.KeySize;
            set
            {
                this.m_rijndael.KeySize = value;
            }
        }

        public override CipherMode Mode
        {
            get => 
                this.m_rijndael.Mode;
            set
            {
                if ((value == CipherMode.CFB) || (value == CipherMode.OFB))
                {
                    throw new CryptographicException(System.SR.GetString("Cryptography_InvalidCipherMode"));
                }
                this.m_rijndael.Mode = value;
            }
        }

        public override PaddingMode Padding
        {
            get => 
                this.m_rijndael.Padding;
            set
            {
                this.m_rijndael.Padding = value;
            }
        }
    }
}

