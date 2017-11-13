namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class SymmetricAlgorithm : IDisposable
    {
        protected int BlockSizeValue;
        protected int FeedbackSizeValue;
        protected byte[] IVValue;
        protected int KeySizeValue;
        protected byte[] KeyValue;
        protected KeySizes[] LegalBlockSizesValue;
        protected KeySizes[] LegalKeySizesValue;
        protected CipherMode ModeValue = CipherMode.CBC;
        protected PaddingMode PaddingValue = PaddingMode.PKCS7;

        protected SymmetricAlgorithm()
        {
        }

        public void Clear()
        {
            ((IDisposable) this).Dispose();
        }

        public static SymmetricAlgorithm Create() => 
            Create("System.Security.Cryptography.SymmetricAlgorithm");

        public static SymmetricAlgorithm Create(string algName) => 
            ((SymmetricAlgorithm) CryptoConfig.CreateFromName(algName));

        public virtual ICryptoTransform CreateDecryptor() => 
            this.CreateDecryptor(this.Key, this.IV);

        public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV);
        public virtual ICryptoTransform CreateEncryptor() => 
            this.CreateEncryptor(this.Key, this.IV);

        public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV);
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.KeyValue != null)
                {
                    Array.Clear(this.KeyValue, 0, this.KeyValue.Length);
                    this.KeyValue = null;
                }
                if (this.IVValue != null)
                {
                    Array.Clear(this.IVValue, 0, this.IVValue.Length);
                    this.IVValue = null;
                }
            }
        }

        public abstract void GenerateIV();
        public abstract void GenerateKey();
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool ValidKeySize(int bitLength)
        {
            KeySizes[] legalKeySizes = this.LegalKeySizes;
            if (legalKeySizes != null)
            {
                for (int i = 0; i < legalKeySizes.Length; i++)
                {
                    if (legalKeySizes[i].SkipSize == 0)
                    {
                        if (legalKeySizes[i].MinSize == bitLength)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        for (int j = legalKeySizes[i].MinSize; j <= legalKeySizes[i].MaxSize; j += legalKeySizes[i].SkipSize)
                        {
                            if (j == bitLength)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public virtual int BlockSize
        {
            get => 
                this.BlockSizeValue;
            set
            {
                for (int i = 0; i < this.LegalBlockSizesValue.Length; i++)
                {
                    if (this.LegalBlockSizesValue[i].SkipSize == 0)
                    {
                        if (this.LegalBlockSizesValue[i].MinSize == value)
                        {
                            this.BlockSizeValue = value;
                            this.IVValue = null;
                            return;
                        }
                    }
                    else
                    {
                        for (int j = this.LegalBlockSizesValue[i].MinSize; j <= this.LegalBlockSizesValue[i].MaxSize; j += this.LegalBlockSizesValue[i].SkipSize)
                        {
                            if (j == value)
                            {
                                if (this.BlockSizeValue != value)
                                {
                                    this.BlockSizeValue = value;
                                    this.IVValue = null;
                                }
                                return;
                            }
                        }
                    }
                }
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidBlockSize"));
            }
        }

        public virtual int FeedbackSize
        {
            get => 
                this.FeedbackSizeValue;
            set
            {
                if (((value <= 0) || (value > this.BlockSizeValue)) || ((value % 8) != 0))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidFeedbackSize"));
                }
                this.FeedbackSizeValue = value;
            }
        }

        public virtual byte[] IV
        {
            get
            {
                if (this.IVValue == null)
                {
                    this.GenerateIV();
                }
                return (byte[]) this.IVValue.Clone();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (value.Length != (this.BlockSizeValue / 8))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidIVSize"));
                }
                this.IVValue = (byte[]) value.Clone();
            }
        }

        public virtual byte[] Key
        {
            get
            {
                if (this.KeyValue == null)
                {
                    this.GenerateKey();
                }
                return (byte[]) this.KeyValue.Clone();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if (!this.ValidKeySize(value.Length * 8))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
                }
                this.KeyValue = (byte[]) value.Clone();
                this.KeySizeValue = value.Length * 8;
            }
        }

        public virtual int KeySize
        {
            get => 
                this.KeySizeValue;
            set
            {
                if (!this.ValidKeySize(value))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
                }
                this.KeySizeValue = value;
                this.KeyValue = null;
            }
        }

        public virtual KeySizes[] LegalBlockSizes =>
            ((KeySizes[]) this.LegalBlockSizesValue.Clone());

        public virtual KeySizes[] LegalKeySizes =>
            ((KeySizes[]) this.LegalKeySizesValue.Clone());

        public virtual CipherMode Mode
        {
            get => 
                this.ModeValue;
            set
            {
                if ((value < CipherMode.CBC) || (CipherMode.CFB < value))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidCipherMode"));
                }
                this.ModeValue = value;
            }
        }

        public virtual PaddingMode Padding
        {
            get => 
                this.PaddingValue;
            set
            {
                if ((value < PaddingMode.None) || (PaddingMode.ISO10126 < value))
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidPaddingMode"));
                }
                this.PaddingValue = value;
            }
        }
    }
}

