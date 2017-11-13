namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class SHA1 : HashAlgorithm
    {
        protected SHA1()
        {
            base.HashSizeValue = 160;
        }

        public static SHA1 Create() => 
            Create("System.Security.Cryptography.SHA1");

        public static SHA1 Create(string hashName) => 
            ((SHA1) CryptoConfig.CreateFromName(hashName));
    }
}

