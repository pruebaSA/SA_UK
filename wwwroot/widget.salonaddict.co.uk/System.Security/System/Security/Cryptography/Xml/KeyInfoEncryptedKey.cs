namespace System.Security.Cryptography.Xml
{
    using System;
    using System.Security.Permissions;
    using System.Xml;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class KeyInfoEncryptedKey : KeyInfoClause
    {
        private System.Security.Cryptography.Xml.EncryptedKey m_encryptedKey;

        public KeyInfoEncryptedKey()
        {
        }

        public KeyInfoEncryptedKey(System.Security.Cryptography.Xml.EncryptedKey encryptedKey)
        {
            this.m_encryptedKey = encryptedKey;
        }

        public override XmlElement GetXml() => 
            this.m_encryptedKey?.GetXml();

        internal override XmlElement GetXml(XmlDocument xmlDocument) => 
            this.m_encryptedKey?.GetXml(xmlDocument);

        public override void LoadXml(XmlElement value)
        {
            this.m_encryptedKey = new System.Security.Cryptography.Xml.EncryptedKey();
            this.m_encryptedKey.LoadXml(value);
        }

        public System.Security.Cryptography.Xml.EncryptedKey EncryptedKey
        {
            get => 
                this.m_encryptedKey;
            set
            {
                this.m_encryptedKey = value;
            }
        }
    }
}

