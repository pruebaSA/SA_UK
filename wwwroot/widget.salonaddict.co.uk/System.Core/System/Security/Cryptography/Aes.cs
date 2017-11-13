namespace System.Security.Cryptography
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public abstract class Aes : SymmetricAlgorithm
    {
        private static KeySizes[] s_legalBlockSizes = new KeySizes[] { new KeySizes(0x80, 0x80, 0) };
        private static KeySizes[] s_legalKeySizes = new KeySizes[] { new KeySizes(0x80, 0x100, 0x40) };

        protected Aes()
        {
            base.LegalBlockSizesValue = s_legalBlockSizes;
            base.LegalKeySizesValue = s_legalKeySizes;
            base.BlockSizeValue = 0x80;
            base.FeedbackSizeValue = 8;
            base.KeySizeValue = 0x100;
            base.ModeValue = CipherMode.CBC;
        }

        public static Aes Create() => 
            Create(typeof(AesCryptoServiceProvider).FullName);

        public static Aes Create(string algorithmName)
        {
            if (algorithmName == null)
            {
                throw new ArgumentNullException("algorithmName");
            }
            return CoreCryptoConfig.CreateFromName<Aes>(algorithmName);
        }
    }
}

