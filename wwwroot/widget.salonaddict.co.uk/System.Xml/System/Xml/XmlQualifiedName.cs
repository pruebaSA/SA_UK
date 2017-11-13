namespace System.Xml
{
    using Microsoft.Win32;
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public class XmlQualifiedName
    {
        public static readonly XmlQualifiedName Empty = new XmlQualifiedName(string.Empty);
        [NonSerialized]
        private int hash;
        private static HashCodeOfStringDelegate hashCodeDelegate = null;
        private string name;
        private string ns;

        public XmlQualifiedName() : this(string.Empty, string.Empty)
        {
        }

        public XmlQualifiedName(string name) : this(name, string.Empty)
        {
        }

        public XmlQualifiedName(string name, string ns)
        {
            this.ns = (ns == null) ? string.Empty : ns;
            this.name = (name == null) ? string.Empty : name;
        }

        internal void Atomize(XmlNameTable nameTable)
        {
            this.name = nameTable.Add(this.name);
            this.ns = nameTable.Add(this.ns);
        }

        internal XmlQualifiedName Clone() => 
            ((XmlQualifiedName) base.MemberwiseClone());

        internal static int Compare(XmlQualifiedName a, XmlQualifiedName b)
        {
            if (null == a)
            {
                if (null != b)
                {
                    return -1;
                }
                return 0;
            }
            if (null == b)
            {
                return 1;
            }
            int num = string.CompareOrdinal(a.Namespace, b.Namespace);
            if (num == 0)
            {
                num = string.CompareOrdinal(a.Name, b.Name);
            }
            return num;
        }

        public override bool Equals(object other)
        {
            if (this == other)
            {
                return true;
            }
            XmlQualifiedName name = other as XmlQualifiedName;
            if (name == null)
            {
                return false;
            }
            return ((this.Name == name.Name) && (this.Namespace == name.Namespace));
        }

        public override int GetHashCode()
        {
            if (this.hash == 0)
            {
                if (hashCodeDelegate == null)
                {
                    hashCodeDelegate = GetHashCodeDelegate();
                }
                this.hash = hashCodeDelegate(this.Name, this.Name.Length, 0L);
            }
            return this.hash;
        }

        [SecuritySafeCritical, ReflectionPermission(SecurityAction.Assert, Unrestricted=true)]
        private static HashCodeOfStringDelegate GetHashCodeDelegate()
        {
            if (!IsRandomizedHashingDisabled())
            {
                MethodInfo method = typeof(string).GetMethod("InternalMarvin32HashString", BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null)
                {
                    return (HashCodeOfStringDelegate) Delegate.CreateDelegate(typeof(HashCodeOfStringDelegate), method);
                }
            }
            return new HashCodeOfStringDelegate(XmlQualifiedName.GetHashCodeOfString);
        }

        private static int GetHashCodeOfString(string s, int length, long additionalEntropy) => 
            s.GetHashCode();

        internal void Init(string name, string ns)
        {
            this.name = name;
            this.ns = ns;
            this.hash = 0;
        }

        [SecuritySafeCritical, RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool IsRandomizedHashingDisabled()
        {
            bool flag = false;
            if (!ReadBoolFromXmlRegistrySettings(Registry.CurrentUser, "DisableRandomizedHashingOnXmlQualifiedName", ref flag))
            {
                ReadBoolFromXmlRegistrySettings(Registry.LocalMachine, "DisableRandomizedHashingOnXmlQualifiedName", ref flag);
            }
            return flag;
        }

        public static bool operator ==(XmlQualifiedName a, XmlQualifiedName b)
        {
            if (a == b)
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            return ((a.Name == b.Name) && (a.Namespace == b.Namespace));
        }

        public static bool operator !=(XmlQualifiedName a, XmlQualifiedName b) => 
            !(a == b);

        internal static XmlQualifiedName Parse(string s, IXmlNamespaceResolver nsmgr, out string prefix)
        {
            string str;
            ValidateNames.ParseQNameThrow(s, out prefix, out str);
            string ns = nsmgr.LookupNamespace(prefix);
            if (ns == null)
            {
                if (prefix.Length != 0)
                {
                    throw new XmlException("Xml_UnknownNs", prefix);
                }
                ns = string.Empty;
            }
            return new XmlQualifiedName(str, ns);
        }

        [SecurityCritical]
        private static bool ReadBoolFromXmlRegistrySettings(RegistryKey hive, string regValueName, ref bool value)
        {
            try
            {
                using (RegistryKey key = hive.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework\XML", false))
                {
                    if ((key != null) && (key.GetValueKind(regValueName) == RegistryValueKind.DWord))
                    {
                        value = ((int) key.GetValue(regValueName)) == 1;
                        return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        internal void SetNamespace(string ns)
        {
            this.ns = ns;
        }

        public override string ToString()
        {
            if (this.Namespace.Length != 0)
            {
                return (this.Namespace + ":" + this.Name);
            }
            return this.Name;
        }

        public static string ToString(string name, string ns)
        {
            if ((ns != null) && (ns.Length != 0))
            {
                return (ns + ":" + name);
            }
            return name;
        }

        internal void Verify()
        {
            XmlConvert.VerifyNCName(this.name);
            if (this.ns.Length != 0)
            {
                XmlConvert.ToUri(this.ns);
            }
        }

        public bool IsEmpty =>
            ((this.Name.Length == 0) && (this.Namespace.Length == 0));

        public string Name =>
            this.name;

        public string Namespace =>
            this.ns;

        private delegate int HashCodeOfStringDelegate(string s, int sLen, long additionalEntropy);
    }
}

