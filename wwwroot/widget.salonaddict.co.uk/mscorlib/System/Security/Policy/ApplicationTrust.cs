namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class ApplicationTrust : ISecurityEncodable
    {
        private System.ApplicationIdentity m_appId;
        private bool m_appTrustedToRun;
        private SecurityElement m_elExtraInfo;
        private object m_extraInfo;
        private StrongName[] m_fullTrustAssemblies;
        [NonSerialized]
        private int m_grantSetSpecialFlags;
        private bool m_persist;
        private PolicyStatement m_psDefaultGrant;

        public ApplicationTrust() : this(new PermissionSet(PermissionState.None))
        {
        }

        public ApplicationTrust(System.ApplicationIdentity applicationIdentity) : this()
        {
            this.ApplicationIdentity = applicationIdentity;
        }

        internal ApplicationTrust(PermissionSet defaultGrantSet) : this(defaultGrantSet, null)
        {
        }

        internal ApplicationTrust(PermissionSet defaultGrantSet, StrongName[] fullTrustAssemblies)
        {
            this.DefaultGrantSet = new PolicyStatement(defaultGrantSet);
            this.FullTrustAssemblies = fullTrustAssemblies;
        }

        public void FromXml(SecurityElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (string.Compare(element.Tag, "ApplicationTrust", StringComparison.Ordinal) != 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidXML"));
            }
            this.m_psDefaultGrant = null;
            this.m_grantSetSpecialFlags = 0;
            this.m_fullTrustAssemblies = null;
            this.m_appTrustedToRun = false;
            string strA = element.Attribute("TrustedToRun");
            if ((strA != null) && (string.Compare(strA, "true", StringComparison.Ordinal) == 0))
            {
                this.m_appTrustedToRun = true;
            }
            string str2 = element.Attribute("Persist");
            if ((str2 != null) && (string.Compare(str2, "true", StringComparison.Ordinal) == 0))
            {
                this.m_persist = true;
            }
            string applicationIdentityFullName = element.Attribute("FullName");
            if ((applicationIdentityFullName != null) && (applicationIdentityFullName.Length > 0))
            {
                this.m_appId = new System.ApplicationIdentity(applicationIdentityFullName);
            }
            SecurityElement element2 = element.SearchForChildByTag("DefaultGrant");
            if (element2 != null)
            {
                SecurityElement et = element2.SearchForChildByTag("PolicyStatement");
                if (et != null)
                {
                    PolicyStatement statement = new PolicyStatement(null);
                    statement.FromXml(et);
                    this.m_psDefaultGrant = statement;
                    this.m_grantSetSpecialFlags = SecurityManager.GetSpecialFlags(statement.PermissionSet, null);
                }
            }
            SecurityElement element4 = element.SearchForChildByTag("FullTrustAssemblies");
            if ((element4 != null) && (element4.InternalChildren != null))
            {
                this.m_fullTrustAssemblies = new StrongName[element4.Children.Count];
                IEnumerator enumerator = element4.Children.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    this.m_fullTrustAssemblies[i] = new StrongName();
                    this.m_fullTrustAssemblies[i].FromXml(enumerator.Current as SecurityElement);
                }
            }
            this.m_elExtraInfo = element.SearchForChildByTag("ExtraInfo");
        }

        private static object ObjectFromXml(SecurityElement elObject)
        {
            if (elObject.Attribute("class") != null)
            {
                ISecurityEncodable encodable = XMLUtil.CreateCodeGroup(elObject) as ISecurityEncodable;
                if (encodable != null)
                {
                    encodable.FromXml(elObject);
                    return encodable;
                }
            }
            MemoryStream serializationStream = new MemoryStream(Hex.DecodeHexString(elObject.Attribute("Data")));
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(serializationStream);
        }

        private static SecurityElement ObjectToXml(string tag, object obj)
        {
            ISecurityEncodable encodable = obj as ISecurityEncodable;
            if ((encodable != null) && !encodable.ToXml().Tag.Equals(tag))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidXML"));
            }
            MemoryStream serializationStream = new MemoryStream();
            new BinaryFormatter().Serialize(serializationStream, obj);
            byte[] sArray = serializationStream.ToArray();
            SecurityElement element = new SecurityElement(tag);
            element.AddAttribute("Data", Hex.EncodeHexString(sArray));
            return element;
        }

        public SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement("ApplicationTrust");
            element.AddAttribute("version", "1");
            if (this.m_appId != null)
            {
                element.AddAttribute("FullName", SecurityElement.Escape(this.m_appId.FullName));
            }
            if (this.m_appTrustedToRun)
            {
                element.AddAttribute("TrustedToRun", "true");
            }
            if (this.m_persist)
            {
                element.AddAttribute("Persist", "true");
            }
            if (this.m_psDefaultGrant != null)
            {
                SecurityElement child = new SecurityElement("DefaultGrant");
                child.AddChild(this.m_psDefaultGrant.ToXml());
                element.AddChild(child);
            }
            if (this.m_fullTrustAssemblies != null)
            {
                SecurityElement element3 = new SecurityElement("FullTrustAssemblies");
                for (int i = 0; i < this.m_fullTrustAssemblies.Length; i++)
                {
                    if (this.m_fullTrustAssemblies[i] != null)
                    {
                        element3.AddChild(this.m_fullTrustAssemblies[i].ToXml());
                    }
                }
                element.AddChild(element3);
            }
            if (this.ExtraInfo != null)
            {
                element.AddChild(ObjectToXml("ExtraInfo", this.ExtraInfo));
            }
            return element;
        }

        public System.ApplicationIdentity ApplicationIdentity
        {
            get => 
                this.m_appId;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(Environment.GetResourceString("Argument_InvalidAppId"));
                }
                this.m_appId = value;
            }
        }

        public PolicyStatement DefaultGrantSet
        {
            get
            {
                if (this.m_psDefaultGrant == null)
                {
                    return new PolicyStatement(new PermissionSet(PermissionState.None));
                }
                return this.m_psDefaultGrant;
            }
            set
            {
                if (value == null)
                {
                    this.m_psDefaultGrant = null;
                    this.m_grantSetSpecialFlags = 0;
                }
                else
                {
                    this.m_psDefaultGrant = value;
                    this.m_grantSetSpecialFlags = SecurityManager.GetSpecialFlags(this.m_psDefaultGrant.PermissionSet, null);
                }
            }
        }

        public object ExtraInfo
        {
            get
            {
                if (this.m_elExtraInfo != null)
                {
                    this.m_extraInfo = ObjectFromXml(this.m_elExtraInfo);
                    this.m_elExtraInfo = null;
                }
                return this.m_extraInfo;
            }
            set
            {
                this.m_elExtraInfo = null;
                this.m_extraInfo = value;
            }
        }

        internal StrongName[] FullTrustAssemblies
        {
            get => 
                this.m_fullTrustAssemblies;
            set
            {
                this.m_fullTrustAssemblies = value;
            }
        }

        public bool IsApplicationTrustedToRun
        {
            get => 
                this.m_appTrustedToRun;
            set
            {
                this.m_appTrustedToRun = value;
            }
        }

        public bool Persist
        {
            get => 
                this.m_persist;
            set
            {
                this.m_persist = value;
            }
        }
    }
}

