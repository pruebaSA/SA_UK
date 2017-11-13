namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class SHA256 : HashAlgorithm
    {
        protected SHA256()
        {
            base.HashSizeValue = 0x100;
        }

        public static SHA256 Create() => 
            Create("System.Security.Cryptography.SHA256");

        public static SHA256 Create(string hashName) => 
            ((SHA256) CryptoConfig.CreateFromName(hashName));
    }
}

