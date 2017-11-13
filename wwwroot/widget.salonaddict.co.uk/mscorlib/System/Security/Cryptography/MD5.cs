namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class MD5 : HashAlgorithm
    {
        protected MD5()
        {
            base.HashSizeValue = 0x80;
        }

        public static MD5 Create() => 
            Create("System.Security.Cryptography.MD5");

        public static MD5 Create(string algName) => 
            ((MD5) CryptoConfig.CreateFromName(algName));
    }
}

