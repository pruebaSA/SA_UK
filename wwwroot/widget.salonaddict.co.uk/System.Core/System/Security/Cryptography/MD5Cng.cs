namespace System.Security.Cryptography
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class MD5Cng : MD5
    {
        private BCryptHashAlgorithm m_hashAlgorithm;

        [SecurityCritical]
        public MD5Cng()
        {
            if (CoreCryptoConfig.EnforceFipsAlgorithms)
            {
                throw new InvalidOperationException(System.SR.GetString("Cryptography_NonCompliantFIPSAlgorithm"));
            }
            this.m_hashAlgorithm = new BCryptHashAlgorithm(CngAlgorithm.MD5, "Microsoft Primitive Provider");
        }

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

