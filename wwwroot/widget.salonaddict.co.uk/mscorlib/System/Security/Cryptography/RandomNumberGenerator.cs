namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class RandomNumberGenerator
    {
        protected RandomNumberGenerator()
        {
        }

        public static RandomNumberGenerator Create() => 
            Create("System.Security.Cryptography.RandomNumberGenerator");

        public static RandomNumberGenerator Create(string rngName) => 
            ((RandomNumberGenerator) CryptoConfig.CreateFromName(rngName));

        public abstract void GetBytes(byte[] data);
        public abstract void GetNonZeroBytes(byte[] data);
    }
}

