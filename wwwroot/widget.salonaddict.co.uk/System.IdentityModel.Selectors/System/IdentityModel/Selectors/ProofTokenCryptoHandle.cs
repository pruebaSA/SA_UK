namespace System.IdentityModel.Selectors
{
    using System;

    internal abstract class ProofTokenCryptoHandle : CryptoHandle
    {
        protected ProofTokenCryptoHandle(InternalRefCountedHandle internalHandle) : base(internalHandle)
        {
        }

        protected ProofTokenCryptoHandle(InternalRefCountedHandle nativeHandle, DateTime expiration, IntPtr nativeParameters, Type paramType) : base(nativeHandle, expiration, nativeParameters, paramType)
        {
        }

        public InfoCardProofToken CreateProofToken()
        {
            base.ThrowIfDisposed();
            return this.OnCreateProofToken();
        }

        protected abstract InfoCardProofToken OnCreateProofToken();
    }
}

