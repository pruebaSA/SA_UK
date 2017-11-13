namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class SHA512 : HashAlgorithm
    {
        protected SHA512()
        {
            base.HashSizeValue = 0x200;
        }

        public static SHA512 Create() => 
            Create("System.Security.Cryptography.SHA512");

        public static SHA512 Create(string hashName) => 
            ((SHA512) CryptoConfig.CreateFromName(hashName));
    }
}

