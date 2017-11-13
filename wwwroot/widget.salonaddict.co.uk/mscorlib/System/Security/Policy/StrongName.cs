namespace System.Security.Policy
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class StrongName : IIdentityPermissionFactory, IBuiltInEvidence, IDelayEvaluatedEvidence
    {
        [NonSerialized]
        private Assembly m_assembly;
        private string m_name;
        private StrongNamePublicKeyBlob m_publicKeyBlob;
        private System.Version m_version;
        [NonSerialized]
        private bool m_wasUsed;

        internal StrongName()
        {
        }

        public StrongName(StrongNamePublicKeyBlob blob, string name, System.Version version) : this(blob, name, version, null)
        {
        }

        internal StrongName(StrongNamePublicKeyBlob blob, string name, System.Version version, Assembly assembly)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyStrongName"));
            }
            if (blob == null)
            {
                throw new ArgumentNullException("blob");
            }
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            this.m_publicKeyBlob = blob;
            this.m_name = name;
            this.m_version = version;
            this.m_assembly = assembly;
        }

        internal static bool CompareNames(string asmName, string mcName)
        {
            if (((mcName.Length > 0) && (mcName[mcName.Length - 1] == '*')) && ((mcName.Length - 1) <= asmName.Length))
            {
                return (string.Compare(mcName, 0, asmName, 0, mcName.Length - 1, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return (string.Compare(mcName, asmName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public object Copy() => 
            new StrongName(this.m_publicKeyBlob, this.m_name, this.m_version);

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new StrongNameIdentityPermission(this.m_publicKeyBlob, this.m_name, this.m_version);

        public override bool Equals(object o)
        {
            StrongName name = o as StrongName;
            return ((((name != null) && object.Equals(this.m_publicKeyBlob, name.m_publicKeyBlob)) && object.Equals(this.m_name, name.m_name)) && object.Equals(this.m_version, name.m_version));
        }

        internal void FromXml(SecurityElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (string.Compare(element.Tag, "StrongName", StringComparison.Ordinal) != 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidXML"));
            }
            this.m_publicKeyBlob = null;
            this.m_version = null;
            string hexString = element.Attribute("Key");
            if (hexString != null)
            {
                this.m_publicKeyBlob = new StrongNamePublicKeyBlob(Hex.DecodeHexString(hexString));
            }
            this.m_name = element.Attribute("Name");
            string version = element.Attribute("Version");
            if (version != null)
            {
                this.m_version = new System.Version(version);
            }
        }

        public override int GetHashCode()
        {
            if (this.m_publicKeyBlob != null)
            {
                return this.m_publicKeyBlob.GetHashCode();
            }
            if ((this.m_name != null) || (this.m_version != null))
            {
                return (((this.m_name == null) ? 0 : this.m_name.GetHashCode()) + ((this.m_version == null) ? 0 : this.m_version.GetHashCode()));
            }
            return typeof(StrongName).GetHashCode();
        }

        internal object Normalize()
        {
            MemoryStream output = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(this.m_publicKeyBlob.PublicKey);
            writer.Write(this.m_version.Major);
            writer.Write(this.m_name);
            output.Position = 0L;
            return output;
        }

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            int num = ((this.m_publicKeyBlob.PublicKey.Length - 1) / 2) + 1;
            if (verbose)
            {
                num += 2;
            }
            num += 8;
            num += this.m_name.Length;
            if (verbose)
            {
                num += 2;
            }
            num++;
            return num;
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            this.m_publicKeyBlob = new StrongNamePublicKeyBlob();
            this.m_publicKeyBlob.PublicKey = new byte[intFromCharArray];
            int num2 = ((intFromCharArray - 1) / 2) + 1;
            Buffer.InternalBlockCopy(buffer, position * 2, this.m_publicKeyBlob.PublicKey, 0, intFromCharArray);
            position += num2;
            int major = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            int minor = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position + 2);
            int build = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position + 4);
            int revision = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position + 6);
            this.m_version = new System.Version(major, minor, build, revision);
            position += 8;
            intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            this.m_name = new string(buffer, position, intFromCharArray);
            return (position + intFromCharArray);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position++] = '\x0002';
            int length = this.m_publicKeyBlob.PublicKey.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, position);
                position += 2;
            }
            Buffer.InternalBlockCopy(this.m_publicKeyBlob.PublicKey, 0, buffer, position * 2, length);
            position += ((length - 1) / 2) + 1;
            BuiltInEvidenceHelper.CopyIntToCharArray(this.m_version.Major, buffer, position);
            BuiltInEvidenceHelper.CopyIntToCharArray(this.m_version.Minor, buffer, position + 2);
            BuiltInEvidenceHelper.CopyIntToCharArray(this.m_version.Build, buffer, position + 4);
            BuiltInEvidenceHelper.CopyIntToCharArray(this.m_version.Revision, buffer, position + 6);
            position += 8;
            int num2 = this.m_name.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(num2, buffer, position);
                position += 2;
            }
            this.m_name.CopyTo(0, buffer, position, num2);
            return (num2 + position);
        }

        void IDelayEvaluatedEvidence.MarkUsed()
        {
            this.m_wasUsed = true;
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("StrongName");
            element.AddAttribute("version", "1");
            if (this.m_publicKeyBlob != null)
            {
                element.AddAttribute("Key", Hex.EncodeHexString(this.m_publicKeyBlob.PublicKey));
            }
            if (this.m_name != null)
            {
                element.AddAttribute("Name", this.m_name);
            }
            if (this.m_version != null)
            {
                element.AddAttribute("Version", this.m_version.ToString());
            }
            return element;
        }

        public string Name =>
            this.m_name;

        public StrongNamePublicKeyBlob PublicKey =>
            this.m_publicKeyBlob;

        bool IDelayEvaluatedEvidence.IsVerified =>
            ((this.m_assembly == null) || this.m_assembly.IsStrongNameVerified());

        bool IDelayEvaluatedEvidence.WasUsed =>
            this.m_wasUsed;

        public System.Version Version =>
            this.m_version;
    }
}

