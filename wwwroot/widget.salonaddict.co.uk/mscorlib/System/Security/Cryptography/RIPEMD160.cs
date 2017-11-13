namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class RIPEMD160 : HashAlgorithm
    {
        protected RIPEMD160()
        {
        }

        public static RIPEMD160 Create() => 
            Create("System.Security.Cryptography.RIPEMD160");

        public static RIPEMD160 Create(string hashName) => 
            ((RIPEMD160) CryptoConfig.CreateFromName(hashName));
    }
}

