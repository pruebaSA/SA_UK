namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class Rijndael : SymmetricAlgorithm
    {
        private static KeySizes[] s_legalBlockSizes = new KeySizes[] { new KeySizes(0x80, 0x100, 0x40) };
        private static KeySizes[] s_legalKeySizes = new KeySizes[] { new KeySizes(0x80, 0x100, 0x40) };

        protected Rijndael()
        {
            base.KeySizeValue = 0x100;
            base.BlockSizeValue = 0x80;
            base.FeedbackSizeValue = base.BlockSizeValue;
            base.LegalBlockSizesValue = s_legalBlockSizes;
            base.LegalKeySizesValue = s_legalKeySizes;
        }

        public static Rijndael Create() => 
            Create("System.Security.Cryptography.Rijndael");

        public static Rijndael Create(string algName) => 
            ((Rijndael) CryptoConfig.CreateFromName(algName));
    }
}

