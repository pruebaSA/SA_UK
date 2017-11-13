namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class DeriveBytes
    {
        protected DeriveBytes()
        {
        }

        public abstract byte[] GetBytes(int cb);
        public abstract void Reset();
    }
}

