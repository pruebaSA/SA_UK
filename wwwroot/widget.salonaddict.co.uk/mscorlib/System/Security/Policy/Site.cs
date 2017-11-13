namespace System.Security.Policy
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class Site : IIdentityPermissionFactory, IBuiltInEvidence
    {
        private SiteString m_name;

        internal Site()
        {
            this.m_name = null;
        }

        public Site(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this.m_name = new SiteString(name);
        }

        internal Site(byte[] id, string name)
        {
            this.m_name = ParseSiteFromUrl(name);
        }

        public object Copy() => 
            new Site(this.Name);

        public static Site CreateFromUrl(string url) => 
            new Site { m_name = ParseSiteFromUrl(url) };

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new SiteIdentityPermission(this.Name);

        public override bool Equals(object o)
        {
            if (!(o is Site))
            {
                return false;
            }
            Site site = (Site) o;
            if (this.Name == null)
            {
                return (site.Name == null);
            }
            return (string.Compare(this.Name, site.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public override int GetHashCode()
        {
            string name = this.Name;
            return name?.GetHashCode();
        }

        internal SiteString GetSiteString() => 
            this.m_name;

        internal object Normalize() => 
            this.m_name.ToString().ToUpper(CultureInfo.InvariantCulture);

        private static SiteString ParseSiteFromUrl(string name)
        {
            URLString str = new URLString(name);
            if (string.Compare(str.Scheme, "file", StringComparison.OrdinalIgnoreCase) == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSite"));
            }
            return new SiteString(new URLString(name).Host);
        }

        int IBuiltInEvidence.GetRequiredSize(bool verbose)
        {
            if (verbose)
            {
                return (this.Name.Length + 3);
            }
            return (this.Name.Length + 1);
        }

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            int intFromCharArray = BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            position += 2;
            this.m_name = new SiteString(new string(buffer, position, intFromCharArray));
            return (position + intFromCharArray);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position++] = '\x0006';
            string name = this.Name;
            int length = name.Length;
            if (verbose)
            {
                BuiltInEvidenceHelper.CopyIntToCharArray(length, buffer, position);
                position += 2;
            }
            name.CopyTo(0, buffer, position, length);
            return (length + position);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.Site");
            element.AddAttribute("version", "1");
            if (this.m_name != null)
            {
                element.AddChild(new SecurityElement("Name", this.m_name.ToString()));
            }
            return element;
        }

        public string Name
        {
            get
            {
                if (this.m_name != null)
                {
                    return this.m_name.ToString();
                }
                return null;
            }
        }
    }
}

