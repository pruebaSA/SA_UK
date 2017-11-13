namespace System.Security.Cryptography.Pkcs
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class PublicKeyInfo
    {
        private AlgorithmIdentifier m_algorithm;
        private byte[] m_keyValue;

        private PublicKeyInfo()
        {
        }

        internal PublicKeyInfo(System.Security.Cryptography.CAPIBase.CERT_PUBLIC_KEY_INFO keyInfo)
        {
            this.m_algorithm = new AlgorithmIdentifier(keyInfo);
            this.m_keyValue = new byte[keyInfo.PublicKey.cbData];
            if (this.m_keyValue.Length > 0)
            {
                Marshal.Copy(keyInfo.PublicKey.pbData, this.m_keyValue, 0, this.m_keyValue.Length);
            }
        }

        public AlgorithmIdentifier Algorithm =>
            this.m_algorithm;

        public byte[] KeyValue =>
            this.m_keyValue;
    }
}

