namespace System.Security.Policy
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class Url : IIdentityPermissionFactory, IBuiltInEvidence
    {
        private URLString m_url;

        internal Url()
        {
            this.m_url = null;
        }

        public Url(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.m_url = new URLString(name);
        }

        internal Url(SerializationInfo info, StreamingContext context)
        {
            this.m_url = new URLString((string) info.GetValue("Url", typeof(string)));
        }

        internal Url(string name, bool parsed)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.m_url = new URLString(name, parsed);
        }

        public object Copy() => 
            new Url { m_url = this.m_url };

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new UrlIdentityPermission(this.m_url);

        public override bool Equals(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (!(o is Url))
            {
                return false;
            }
            Url url = (Url) o;
            if (this.m_url == null)
            {
                return (url.m_url == null);
            }
            if (url.m_url == null)
            {
                return false;
            }
            return this.m_url.Equals(url.m_url);
        }

        public override int GetHashCode() => 
            this.m_url?.GetHashCode();

        internal URLString GetURLString() => 
            this.m_url;

        internal object Normalize() => 
            this.m_url.NormalizeUrl();

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            if (verbose)
            {
                return (this.Value.Length + 3);
            }
            return (this.Value.Length + 1);
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            this.m_url = new URLString(new string(buffer, position, intFromCharArray));
            return (position + intFromCharArray);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position++] = '\x0004';
            string str = this.Value;
            int length = str.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, position);
                position += 2;
            }
            str.CopyTo(0, buffer, position, length);
            return (length + position);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.Url");
            element.AddAttribute("version", "1");
            if (this.m_url != null)
            {
                element.AddChild(new SecurityElement("Url", this.m_url.ToString()));
            }
            return element;
        }

        public string Value =>
            this.m_url?.ToString();
    }
}

