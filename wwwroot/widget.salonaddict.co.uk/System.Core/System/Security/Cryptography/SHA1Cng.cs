namespace System.Security.Cryptography
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class SHA1Cng : SHA1
    {
        private BCryptHashAlgorithm m_hashAlgorithm = new BCryptHashAlgorithm(CngAlgorithm.Sha1, "Microsoft Primitive Provider");

        [SecurityCritical]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.m_hashAlgorithm.Dispose();
            }
        }

        [SecurityCritical]
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            this.m_hashAlgorithm.HashCore(array, ibStart, cbSize);
        }

        [SecurityCritical]
        protected override byte[] HashFinal() => 
            this.m_hashAlgorithm.HashFinal();

        [SecurityCritical]
        public override void Initialize()
        {
            this.m_hashAlgorithm.Initialize();
        }
    }
}

