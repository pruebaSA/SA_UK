namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class SHA384 : HashAlgorithm
    {
        protected SHA384()
        {
            base.HashSizeValue = 0x180;
        }

        public static SHA384 Create() => 
            Create("System.Security.Cryptography.SHA384");

        public static SHA384 Create(string hashName) => 
            ((SHA384) CryptoConfig.CreateFromName(hashName));
    }
}

