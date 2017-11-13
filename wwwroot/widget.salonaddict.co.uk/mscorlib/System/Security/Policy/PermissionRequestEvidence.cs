namespace System.Security.Policy
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class PermissionRequestEvidence : IBuiltInEvidence
    {
        private const char idDenied = '\x0002';
        private const char idOptional = '\x0001';
        private const char idRequest = '\0';
        private PermissionSet m_denied;
        private PermissionSet m_optional;
        private PermissionSet m_request;
        private string m_strDenied;
        private string m_strOptional;
        private string m_strRequest;

        internal PermissionRequestEvidence()
        {
        }

        public PermissionRequestEvidence(PermissionSet request, PermissionSet optional, PermissionSet denied)
        {
            if (request == null)
            {
                this.m_request = null;
            }
            else
            {
                this.m_request = request.Copy();
            }
            if (optional == null)
            {
                this.m_optional = null;
            }
            else
            {
                this.m_optional = optional.Copy();
            }
            if (denied == null)
            {
                this.m_denied = null;
            }
            else
            {
                this.m_denied = denied.Copy();
            }
        }

        public PermissionRequestEvidence Copy() => 
            new PermissionRequestEvidence(this.m_request, this.m_optional, this.m_denied);

        internal void CreateStrings()
        {
            if ((this.m_strRequest == null) && (this.m_request != null))
            {
                this.m_strRequest = this.m_request.ToXml().ToString();
            }
            if ((this.m_strOptional == null) && (this.m_optional != null))
            {
                this.m_strOptional = this.m_optional.ToXml().ToString();
            }
            if ((this.m_strDenied == null) && (this.m_denied != null))
            {
                this.m_strDenied = this.m_denied.ToXml().ToString();
            }
        }

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            this.CreateStrings();
            int num = 1;
            if (this.m_strRequest != null)
            {
                if (verbose)
                {
                    num += 3;
                }
                num += this.m_strRequest.Length;
            }
            if (this.m_strOptional != null)
            {
                if (verbose)
                {
                    num += 3;
                }
                num += this.m_strOptional.Length;
            }
            if (this.m_strDenied != null)
            {
                if (verbose)
                {
                    num += 3;
                }
                num += this.m_strDenied.Length;
            }
            if (verbose)
            {
                num += 2;
            }
            return num;
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            for (int i = 0; i < intFromCharArray; i++)
            {
                char ch = buffer[position++];
                int length = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
                position += 2;
                string input = new string(buffer, position, length);
                position += length;
                Parser parser = new Parser(input);
                PermissionSet set = new PermissionSet();
                set.FromXml(parser.GetTopElement());
                switch (ch)
                {
                    case '\0':
                        this.m_strRequest = input;
                        this.m_request = set;
                        break;

                    case '\x0001':
                        this.m_strOptional = input;
                        this.m_optional = set;
                        break;

                    case '\x0002':
                        this.m_strDenied = input;
                        this.m_denied = set;
                        break;

                    default:
                        throw new SerializationException(Environment.GetResourceString("Serialization_UnableToFixup"));
                }
            }
            return position;
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            int length;
            this.CreateStrings();
            int num = position;
            int num2 = 0;
            int num3 = 0;
            buffer[num++] = '\a';
            if (verbose)
            {
                num2 = num;
                num += 2;
            }
            if (this.m_strRequest != null)
            {
                length = this.m_strRequest.Length;
                if (verbose)
                {
                    buffer[num++] = '\0';
                    BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, num);
                    num += 2;
                    num3++;
                }
                this.m_strRequest.CopyTo(0, buffer, num, length);
                num += length;
            }
            if (this.m_strOptional != null)
            {
                length = this.m_strOptional.Length;
                if (verbose)
                {
                    buffer[num++] = '\x0001';
                    BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, num);
                    num += 2;
                    num3++;
                }
                this.m_strOptional.CopyTo(0, buffer, num, length);
                num += length;
            }
            if (this.m_strDenied != null)
            {
                length = this.m_strDenied.Length;
                if (verbose)
                {
                    buffer[num++] = '\x0002';
                    BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, num);
                    num += 2;
                    num3++;
                }
                this.m_strDenied.CopyTo(0, buffer, num, length);
                num += length;
            }
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(num3, buffer, num2);
            }
            return num;
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element2;
            SecurityElement element = new SecurityElement("System.Security.Policy.PermissionRequestEvidence");
            element.AddAttribute("version", "1");
            if (this.m_request != null)
            {
                element2 = new SecurityElement("Request");
                element2.AddChild(this.m_request.ToXml());
                element.AddChild(element2);
            }
            if (this.m_optional != null)
            {
                element2 = new SecurityElement("Optional");
                element2.AddChild(this.m_optional.ToXml());
                element.AddChild(element2);
            }
            if (this.m_denied != null)
            {
                element2 = new SecurityElement("Denied");
                element2.AddChild(this.m_denied.ToXml());
                element.AddChild(element2);
            }
            return element;
        }

        public PermissionSet DeniedPermissions =>
            this.m_denied;

        public PermissionSet OptionalPermissions =>
            this.m_optional;

        public PermissionSet RequestedPermissions =>
            this.m_request;
    }
}

