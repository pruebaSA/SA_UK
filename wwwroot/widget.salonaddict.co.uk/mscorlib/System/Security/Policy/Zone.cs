namespace System.Security.Policy
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class Zone : IIdentityPermissionFactory, IBuiltInEvidence
    {
        [OptionalField(VersionAdded=2)]
        private string m_url;
        private System.Security.SecurityZone m_zone;
        private static readonly string[] s_names = new string[] { "MyComputer", "Intranet", "Trusted", "Internet", "Untrusted", "NoZone" };

        internal Zone()
        {
            this.m_url = null;
            this.m_zone = System.Security.SecurityZone.NoZone;
        }

        public Zone(System.Security.SecurityZone zone)
        {
            if ((zone < System.Security.SecurityZone.NoZone) || (zone > System.Security.SecurityZone.Untrusted))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_IllegalZone"));
            }
            this.m_url = null;
            this.m_zone = zone;
        }

        private Zone(string url)
        {
            this.m_url = url;
            this.m_zone = System.Security.SecurityZone.NoZone;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern System.Security.SecurityZone _CreateFromUrl(string url);
        public object Copy() => 
            new Zone { 
                m_zone = this.m_zone,
                m_url = this.m_url
            };

        public static Zone CreateFromUrl(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            return new Zone(url);
        }

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new ZoneIdentityPermission(this.SecurityZone);

        public override bool Equals(object o)
        {
            if (o is Zone)
            {
                Zone zone = (Zone) o;
                return (this.SecurityZone == zone.SecurityZone);
            }
            return false;
        }

        public override int GetHashCode() => 
            ((int) this.SecurityZone);

        internal object Normalize() => 
            s_names[(int) this.SecurityZone];

        int IBuiltInEvidence.GetRequiredSize(bool verbose) => 
            3;

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
        {
            this.m_url = null;
            this.m_zone = (System.Security.SecurityZone) BuiltInEvidenceHelper.GetIntFromCharArray(buffer, position);
            return (position + 2);
        }

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position] = '\x0003';
            BuiltInEvidenceHelper.CopyIntToCharArray((int) this.SecurityZone, buffer, position + 1);
            return (position + 3);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("System.Security.Policy.Zone");
            element.AddAttribute("version", "1");
            if (this.SecurityZone != System.Security.SecurityZone.NoZone)
            {
                element.AddChild(new SecurityElement("Zone", s_names[(int) this.SecurityZone]));
                return element;
            }
            element.AddChild(new SecurityElement("Zone", s_names[s_names.Length - 1]));
            return element;
        }

        public System.Security.SecurityZone SecurityZone
        {
            get
            {
                if (this.m_url != null)
                {
                    this.m_zone = _CreateFromUrl(this.m_url);
                }
                return this.m_zone;
            }
        }
    }
}

