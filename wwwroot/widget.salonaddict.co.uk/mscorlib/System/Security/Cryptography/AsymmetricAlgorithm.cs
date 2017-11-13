namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class AsymmetricAlgorithm : IDisposable
    {
        protected int KeySizeValue;
        protected KeySizes[] LegalKeySizesValue;

        protected AsymmetricAlgorithm()
        {
        }

        public void Clear()
        {
            ((IDisposable) this).Dispose();
        }

        public static AsymmetricAlgorithm Create() => 
            Create("System.Security.Cryptography.AsymmetricAlgorithm");

        public static AsymmetricAlgorithm Create(string algName) => 
            ((AsymmetricAlgorithm) CryptoConfig.CreateFromName(algName));

        protected abstract void Dispose(bool disposing);
        public abstract void FromXmlString(string xmlString);
        void IDisposable.Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract string ToXmlString(bool includePrivateParameters);

        public abstract string KeyExchangeAlgorithm { get; }

        public virtual int KeySize
        {
            get => 
                this.KeySizeValue;
            set
            {
                for (int i = 0; i < this.LegalKeySizesValue.Length; i++)
                {
                    if (this.LegalKeySizesValue[i].SkipSize == 0)
                    {
                        if (this.LegalKeySizesValue[i].MinSize == value)
                        {
                            this.KeySizeValue = value;
                            return;
                        }
                    }
                    else
                    {
                        for (int j = this.LegalKeySizesValue[i].MinSize; j <= this.LegalKeySizesValue[i].MaxSize; j += this.LegalKeySizesValue[i].SkipSize)
                        {
                            if (j == value)
                            {
                                this.KeySizeValue = value;
                                return;
                            }
                        }
                    }
                }
                throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
            }
        }

        public virtual KeySizes[] LegalKeySizes =>
            ((KeySizes[]) this.LegalKeySizesValue.Clone());

        public abstract string SignatureAlgorithm { get; }
    }
}

