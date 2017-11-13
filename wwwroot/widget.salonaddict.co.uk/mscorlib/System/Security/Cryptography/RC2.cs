namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class RC2 : SymmetricAlgorithm
    {
        protected int EffectiveKeySizeValue;
        private static KeySizes[] s_legalBlockSizes = new KeySizes[] { new KeySizes(0x40, 0x40, 0) };
        private static KeySizes[] s_legalKeySizes = new KeySizes[] { new KeySizes(40, 0x400, 8) };

        protected RC2()
        {
            base.KeySizeValue = 0x80;
            base.BlockSizeValue = 0x40;
            base.FeedbackSizeValue = base.BlockSizeValue;
            base.LegalBlockSizesValue = s_legalBlockSizes;
            base.LegalKeySizesValue = s_legalKeySizes;
        }

        public static RC2 Create() => 
            Create("System.Security.Cryptography.RC2");

        public static RC2 Create(string AlgName) => 
            ((RC2) CryptoConfig.CreateFromName(AlgName));

        public virtual int EffectiveKeySize
        {
            get
            {
                if (this.EffectiveKeySizeValue == 0)
                {
                    return base.KeySizeValue;
                }
                return this.EffectiveKeySizeValue;
            }
            set
            {
                if (value > base.KeySizeValue)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_RC2_EKSKS"));
                }
                if (value == 0)
                {
                    this.EffectiveKeySizeValue = value;
                }
                else
                {
                    if (value < 40)
                    {
                        throw new CryptographicException(Environment.GetResourceString("Cryptography_RC2_EKS40"));
                    }
                    if (!base.ValidKeySize(value))
                    {
                        throw new CryptographicException(Environment.GetResourceString("Cryptography_InvalidKeySize"));
                    }
                    this.EffectiveKeySizeValue = value;
                }
            }
        }

        public override int KeySize
        {
            get => 
                base.KeySizeValue;
            set
            {
                if (value < this.EffectiveKeySizeValue)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_RC2_EKSKS"));
                }
                base.KeySize = value;
            }
        }
    }
}

