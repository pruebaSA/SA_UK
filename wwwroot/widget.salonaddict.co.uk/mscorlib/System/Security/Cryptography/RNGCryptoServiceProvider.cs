namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class RNGCryptoServiceProvider : RandomNumberGenerator
    {
        private SafeProvHandle m_safeProvHandle;

        public RNGCryptoServiceProvider() : this((CspParameters) null)
        {
        }

        public RNGCryptoServiceProvider(string str) : this((CspParameters) null)
        {
        }

        public RNGCryptoServiceProvider(byte[] rgb) : this((CspParameters) null)
        {
        }

        public RNGCryptoServiceProvider(CspParameters cspParams)
        {
            if (cspParams != null)
            {
                this.m_safeProvHandle = Utils.AcquireProvHandle(cspParams);
            }
            else
            {
                this.m_safeProvHandle = Utils.StaticProvHandle;
            }
        }

        public override void GetBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Utils._GetBytes(this.m_safeProvHandle, data);
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            Utils._GetNonZeroBytes(this.m_safeProvHandle, data);
        }
    }
}

