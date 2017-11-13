namespace System.Security.Policy
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class Publisher : IIdentityPermissionFactory, IBuiltInEvidence
    {
        private X509Certificate m_cert;

        internal Publisher()
        {
        }

        public Publisher(X509Certificate cert)
        {
            if (cert == null)
            {
                throw new ArgumentNullException("cert");
            }
            this.m_cert = cert;
        }

        public object Copy()
        {
            Publisher publisher = new Publisher();
            if (this.m_cert != null)
            {
                publisher.m_cert = new X509Certificate(this.m_cert);
            }
            return publisher;
        }

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new PublisherIdentityPermission(this.m_cert);

        public override bool Equals(object o)
        {
            Publisher publisher = o as Publisher;
            return ((publisher != null) && PublicKeyEquals(this.m_cert, publisher.m_cert));
        }

        public override int GetHashCode() => 
            this.m_cert.GetHashCode();

        internal object Normalize() => 
            new MemoryStream(this.m_cert.GetRawCertData()) { Position = 0L };

        internal static bool PublicKeyEquals(X509Certificate cert1, X509Certificate cert2)
        {
            if (cert1 == null)
            {
                return (cert2 == null);
            }
            if (cert2 == null)
            {
                return false;
            }
            byte[] publicKey = cert1.GetPublicKey();
            string keyAlgorithm = cert1.GetKeyAlgorithm();
            byte[] keyAlgorithmParameters = cert1.GetKeyAlgorithmParameters();
            byte[] buffer3 = cert2.GetPublicKey();
            string str2 = cert2.GetKeyAlgorithm();
            byte[] buffer4 = cert2.GetKeyAlgorithmParameters();
            int length = publicKey.Length;
            if (length != buffer3.Length)
            {
                return false;
            }
            for (int i = 0; i < length; i++)
            {
                if (publicKey[i] != buffer3[i])
                {
                    return false;
                }
            }
            if (!keyAlgorithm.Equals(str2))
            {
                return false;
            }
            length = keyAlgorithmParameters.Length;
            if (buffer4.Length != length)
            {
                return false;
            }
            for (int j = 0; j < length; j++)
            {
                if (keyAlgorithmParameters[j] != buffer4[j])
                {
                    return false;
                }
            }
            return true;
        }

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            int num = ((this.Certificate.GetRawCertData().Length - 1) / 2) + 1;
            if (verbose)
            {
                return (num + 3);
            }
            return (num + 1);
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            byte[] dst = new byte[intFromCharArray];
            int num2 = ((intFromCharArray - 1) / 2) + 1;
            Buffer.InternalBlockCopy(buffer, position * 2, dst, 0, intFromCharArray);
            this.m_cert = new X509Certificate(dst);
            return (position + num2);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position++] = '\x0001';
            byte[] rawCertData = this.Certificate.GetRawCertData();
            int length = rawCertData.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, position);
                position += 2;
            }
            Buffer.InternalBlockCopy(rawCertData, 0, buffer, position * 2, length);
            return ((((length - 1) / 2) + 1) + position);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.Publisher");
            element.AddAttribute("version", "1");
            element.AddChild(new SecurityElement("X509v3Certificate", (this.m_cert != null) ? this.m_cert.GetRawCertDataString() : ""));
            return element;
        }

        public X509Certificate Certificate
        {
            get
            {
                if (this.m_cert == null)
                {
                    return null;
                }
                return new X509Certificate(this.m_cert);
            }
        }
    }
}

