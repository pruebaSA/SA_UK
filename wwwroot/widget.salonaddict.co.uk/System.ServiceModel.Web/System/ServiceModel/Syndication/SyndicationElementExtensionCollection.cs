namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    public sealed class SyndicationElementExtensionCollection : Collection<SyndicationElementExtension>
    {
        private XmlBuffer buffer;
        private bool initialized;

        internal SyndicationElementExtensionCollection() : this((XmlBuffer) null)
        {
        }

        internal SyndicationElementExtensionCollection(SyndicationElementExtensionCollection source)
        {
            this.buffer = source.buffer;
            for (int i = 0; i < source.Items.Count; i++)
            {
                base.Add(source.Items[i]);
            }
            this.initialized = true;
        }

        internal SyndicationElementExtensionCollection(XmlBuffer buffer)
        {
            this.buffer = buffer;
            if (this.buffer != null)
            {
                this.PopulateElements();
            }
            this.initialized = true;
        }

        public void Add(object extension)
        {
            if (extension is SyndicationElementExtension)
            {
                base.Add((SyndicationElementExtension) extension);
            }
            else
            {
                this.Add(extension, (DataContractSerializer) null);
            }
        }

        public void Add(XmlReader xmlReader)
        {
            if (xmlReader == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xmlReader");
            }
            base.Add(new SyndicationElementExtension(xmlReader));
        }

        public void Add(object dataContractExtension, DataContractSerializer serializer)
        {
            this.Add(null, null, dataContractExtension, serializer);
        }

        public void Add(object xmlSerializerExtension, XmlSerializer serializer)
        {
            if (xmlSerializerExtension == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xmlSerializerExtension");
            }
            if (serializer == null)
            {
                serializer = new XmlSerializer(xmlSerializerExtension.GetType());
            }
            base.Add(new SyndicationElementExtension(xmlSerializerExtension, serializer));
        }

        public void Add(string outerName, string outerNamespace, object dataContractExtension)
        {
            this.Add(outerName, outerNamespace, dataContractExtension, null);
        }

        public void Add(string outerName, string outerNamespace, object dataContractExtension, XmlObjectSerializer dataContractSerializer)
        {
            if (dataContractExtension == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dataContractExtension");
            }
            if (dataContractSerializer == null)
            {
                dataContractSerializer = new DataContractSerializer(dataContractExtension.GetType());
            }
            base.Add(new SyndicationElementExtension(outerName, outerNamespace, dataContractExtension, dataContractSerializer));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            if (this.initialized)
            {
                this.buffer = null;
            }
        }

        private XmlBuffer GetOrCreateBufferOverExtensions()
        {
            if (this.buffer != null)
            {
                return this.buffer;
            }
            XmlBuffer buffer = new XmlBuffer(0x7fffffff);
            using (XmlWriter writer = buffer.OpenSection(XmlDictionaryReaderQuotas.Max))
            {
                writer.WriteStartElement("extensionWrapper");
                for (int i = 0; i < base.Count; i++)
                {
                    base[i].WriteTo(writer);
                }
                writer.WriteEndElement();
            }
            buffer.CloseSection();
            buffer.Close();
            this.buffer = buffer;
            return buffer;
        }

        public XmlReader GetReaderAtElementExtensions()
        {
            XmlReader reader = this.GetOrCreateBufferOverExtensions().GetReader(0);
            reader.ReadStartElement();
            return reader;
        }

        protected override void InsertItem(int index, SyndicationElementExtension item)
        {
            if (item == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            base.InsertItem(index, item);
            if (this.initialized)
            {
                this.buffer = null;
            }
        }

        private void PopulateElements()
        {
            using (XmlDictionaryReader reader = this.buffer.GetReader(0))
            {
                reader.ReadStartElement();
                for (int i = 0; reader.IsStartElement(); i++)
                {
                    base.Add(new SyndicationElementExtension(this.buffer, i, reader.LocalName, reader.NamespaceURI));
                    reader.Skip();
                }
            }
        }

        public Collection<TExtension> ReadElementExtensions<TExtension>(string extensionName, string extensionNamespace) => 
            this.ReadElementExtensions<TExtension>(extensionName, extensionNamespace, new DataContractSerializer(typeof(TExtension)));

        public Collection<TExtension> ReadElementExtensions<TExtension>(string extensionName, string extensionNamespace, XmlObjectSerializer serializer)
        {
            if (serializer == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
            }
            return this.ReadExtensions<TExtension>(extensionName, extensionNamespace, serializer, null);
        }

        public Collection<TExtension> ReadElementExtensions<TExtension>(string extensionName, string extensionNamespace, XmlSerializer serializer)
        {
            if (serializer == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
            }
            return this.ReadExtensions<TExtension>(extensionName, extensionNamespace, null, serializer);
        }

        private Collection<TExtension> ReadExtensions<TExtension>(string extensionName, string extensionNamespace, XmlObjectSerializer dcSerializer, XmlSerializer xmlSerializer)
        {
            if (string.IsNullOrEmpty(extensionName))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR2.GetString(SR2.ExtensionNameNotSpecified, new object[0]));
            }
            if (extensionNamespace == null)
            {
                extensionNamespace = string.Empty;
            }
            Collection<TExtension> collection = new Collection<TExtension>();
            for (int i = 0; i < base.Count; i++)
            {
                if ((extensionName == base[i].OuterName) && (extensionNamespace == base[i].OuterNamespace))
                {
                    if (dcSerializer != null)
                    {
                        collection.Add(base[i].GetObject<TExtension>(dcSerializer));
                    }
                    else
                    {
                        collection.Add(base[i].GetObject<TExtension>(xmlSerializer));
                    }
                }
            }
            return collection;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            if (this.initialized)
            {
                this.buffer = null;
            }
        }

        protected override void SetItem(int index, SyndicationElementExtension item)
        {
            if (item == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
            }
            base.SetItem(index, item);
            if (this.initialized)
            {
                this.buffer = null;
            }
        }

        internal void WriteTo(XmlWriter writer)
        {
            if (this.buffer != null)
            {
                using (XmlDictionaryReader reader = this.buffer.GetReader(0))
                {
                    reader.ReadStartElement();
                    while (reader.IsStartElement())
                    {
                        writer.WriteNode(reader, false);
                    }
                    return;
                }
            }
            for (int i = 0; i < base.Items.Count; i++)
            {
                base.Items[i].WriteTo(writer);
            }
        }
    }
}

