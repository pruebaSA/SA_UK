namespace System.Security.Cryptography
{
    using Microsoft.Contracts;
    using System;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class ECDiffieHellmanCngPublicKey : ECDiffieHellmanPublicKey
    {
        private CngKeyBlobFormat m_format;
        [NonSerialized]
        private CngKey m_key;

        [SecurityCritical]
        internal ECDiffieHellmanCngPublicKey(CngKey key) : base(key.Export(CngKeyBlobFormat.EccPublicBlob))
        {
            Contract.Requires((key != null) && (key.AlgorithmGroup == CngAlgorithmGroup.ECDiffieHellman));
            this.m_format = CngKeyBlobFormat.EccPublicBlob;
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
            this.m_key = CngKey.Open(key.Handle, key.IsEphemeral ? CngKeyHandleOpenOptions.EphemeralKey : CngKeyHandleOpenOptions.None);
            CodeAccessPermission.RevertAssert();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && (this.m_key != null))
            {
                this.m_key.Dispose();
            }
        }

        [SecurityCritical]
        public static ECDiffieHellmanPublicKey FromByteArray(byte[] publicKeyBlob, CngKeyBlobFormat format)
        {
            if (publicKeyBlob == null)
            {
                throw new ArgumentNullException("publicKeyBlob");
            }
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            using (CngKey key = CngKey.Import(publicKeyBlob, format))
            {
                if (key.AlgorithmGroup != CngAlgorithmGroup.ECDiffieHellman)
                {
                    throw new ArgumentException(System.SR.GetString("Cryptography_ArgECDHRequiresECDHKey"));
                }
                return new ECDiffieHellmanCngPublicKey(key);
            }
        }

        [SecurityCritical]
        public static ECDiffieHellmanCngPublicKey FromXmlString(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }
            using (CngKey key = Rfc4050KeyFormatter.FromXml(xml))
            {
                if (key.AlgorithmGroup != CngAlgorithmGroup.ECDiffieHellman)
                {
                    throw new ArgumentException(System.SR.GetString("Cryptography_ArgECDHRequiresECDHKey"), "xml");
                }
                return new ECDiffieHellmanCngPublicKey(key);
            }
        }

        public CngKey Import() => 
            CngKey.Import(this.ToByteArray(), this.BlobFormat);

        public override string ToXmlString()
        {
            if (this.m_key == null)
            {
                this.m_key = this.Import();
            }
            return Rfc4050KeyFormatter.ToXml(this.m_key);
        }

        public CngKeyBlobFormat BlobFormat =>
            this.m_format;
    }
}

