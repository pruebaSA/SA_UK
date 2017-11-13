namespace System.Configuration
{
    using System;
    using System.Configuration.Provider;
    using System.Xml;

    public abstract class ProtectedConfigurationProvider : ProviderBase
    {
        protected ProtectedConfigurationProvider()
        {
        }

        public abstract XmlNode Decrypt(XmlNode encryptedNode);
        public abstract XmlNode Encrypt(XmlNode node);
    }
}

