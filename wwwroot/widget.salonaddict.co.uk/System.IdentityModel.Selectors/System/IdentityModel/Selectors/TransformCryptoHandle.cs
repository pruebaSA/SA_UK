namespace System.IdentityModel.Selectors
{
    using System;

    internal class TransformCryptoHandle : CryptoHandle
    {
        private TransformCryptoHandle(InternalRefCountedHandle internalHandle) : base(internalHandle)
        {
        }

        public TransformCryptoHandle(InternalRefCountedHandle nativeHandle, DateTime expiration, IntPtr parameters) : base(nativeHandle, expiration, parameters, typeof(RpcTransformCryptoParameters))
        {
        }

        protected override CryptoHandle OnDuplicate() => 
            new TransformCryptoHandle(base.InternalHandle);
    }
}

