namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public sealed class MD5CryptoServiceProvider : MD5
    {
        private SafeHashHandle _safeHashHandle;

        public MD5CryptoServiceProvider()
        {
            if (Utils.FipsAlgorithmPolicy == 1)
            {
                throw new InvalidOperationException(Environment.GetResourceString("Cryptography_NonCompliantFIPSAlgorithm"));
            }
            SafeHashHandle invalidHandle = SafeHashHandle.InvalidHandle;
            Utils._CreateHash(Utils.StaticProvHandle, 0x8003, ref invalidHandle);
            this._safeHashHandle = invalidHandle;
        }

        protected override void Dispose(bool disposing)
        {
            if ((this._safeHashHandle != null) && !this._safeHashHandle.IsClosed)
            {
                this._safeHashHandle.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            Utils._HashData(this._safeHashHandle, rgb, ibStart, cbSize);
        }

        protected override byte[] HashFinal() => 
            Utils._EndHash(this._safeHashHandle);

        public override void Initialize()
        {
            if ((this._safeHashHandle != null) && !this._safeHashHandle.IsClosed)
            {
                this._safeHashHandle.Dispose();
            }
            SafeHashHandle invalidHandle = SafeHashHandle.InvalidHandle;
            Utils._CreateHash(Utils.StaticProvHandle, 0x8003, ref invalidHandle);
            this._safeHashHandle = invalidHandle;
        }
    }
}

