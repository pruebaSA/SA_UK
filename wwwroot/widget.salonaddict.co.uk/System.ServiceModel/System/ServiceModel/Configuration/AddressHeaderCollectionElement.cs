namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel.Channels;
    using System.Xml;

    public sealed class AddressHeaderCollectionElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [SecurityTreatAsSafe, SecurityCritical]
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            this.SetIsPresent();
            this.DeserializeElementCore(reader);
        }

        private void DeserializeElementCore(XmlReader reader)
        {
            this.Headers = AddressHeaderCollection.ReadServiceParameters(XmlDictionaryReader.CreateDictionaryReader(reader));
        }

        protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
        {
            bool flag = this.Headers.Count != 0;
            if (flag && (writer != null))
            {
                writer.WriteStartElement(elementName);
                this.Headers.WriteContentsTo(XmlDictionaryWriter.CreateDictionaryWriter(writer));
                writer.WriteEndElement();
            }
            return flag;
        }

        [SecurityCritical]
        private void SetIsPresent()
        {
            ConfigurationHelpers.SetIsPresent(this);
        }

        [ConfigurationProperty("headers", DefaultValue=null)]
        public AddressHeaderCollection Headers
        {
            get
            {
                AddressHeaderCollection emptyHeaderCollection = (AddressHeaderCollection) base["headers"];
                if (emptyHeaderCollection == null)
                {
                    emptyHeaderCollection = AddressHeaderCollection.EmptyHeaderCollection;
                }
                return emptyHeaderCollection;
            }
            set
            {
                if (value == null)
                {
                    value = AddressHeaderCollection.EmptyHeaderCollection;
                }
                base["headers"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("headers", typeof(AddressHeaderCollection), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

