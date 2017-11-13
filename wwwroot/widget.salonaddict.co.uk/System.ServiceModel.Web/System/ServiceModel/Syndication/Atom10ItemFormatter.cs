namespace System.ServiceModel.Syndication
{
    using System;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="entry", Namespace="http://www.w3.org/2005/Atom")]
    public class Atom10ItemFormatter : SyndicationItemFormatter, IXmlSerializable
    {
        private Atom10FeedFormatter feedSerializer;
        private Type itemType;
        private bool preserveAttributeExtensions;
        private bool preserveElementExtensions;

        public Atom10ItemFormatter() : this(typeof(SyndicationItem))
        {
        }

        public Atom10ItemFormatter(SyndicationItem itemToWrite) : base(itemToWrite)
        {
            this.feedSerializer = new Atom10FeedFormatter();
            this.feedSerializer.PreserveAttributeExtensions = this.preserveAttributeExtensions = true;
            this.feedSerializer.PreserveElementExtensions = this.preserveElementExtensions = true;
            this.itemType = itemToWrite.GetType();
        }

        public Atom10ItemFormatter(Type itemTypeToCreate)
        {
            if (itemTypeToCreate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("itemTypeToCreate");
            }
            if (!typeof(SyndicationItem).IsAssignableFrom(itemTypeToCreate))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("itemTypeToCreate", SR2.GetString(SR2.InvalidObjectTypePassed, new object[] { "itemTypeToCreate", "SyndicationItem" }));
            }
            this.feedSerializer = new Atom10FeedFormatter();
            this.feedSerializer.PreserveAttributeExtensions = this.preserveAttributeExtensions = true;
            this.feedSerializer.PreserveElementExtensions = this.preserveElementExtensions = true;
            this.itemType = itemTypeToCreate;
        }

        public override bool CanRead(XmlReader reader) => 
            reader?.IsStartElement("entry", "http://www.w3.org/2005/Atom");

        protected override SyndicationItem CreateItemInstance() => 
            SyndicationItemFormatter.CreateItemInstance(this.itemType);

        public override void ReadFrom(XmlReader reader)
        {
            SyndicationFeedFormatter.TraceItemReadBegin();
            if (!this.CanRead(reader))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.UnknownItemXml, new object[] { reader.LocalName, reader.NamespaceURI })));
            }
            this.ReadItem(reader);
            SyndicationFeedFormatter.TraceItemReadEnd();
        }

        private void ReadItem(XmlReader reader)
        {
            this.SetItem(this.CreateItemInstance());
            this.feedSerializer.ReadItemFrom(XmlDictionaryReader.CreateDictionaryReader(reader), base.Item);
        }

        XmlSchema IXmlSerializable.GetSchema() => 
            null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            SyndicationFeedFormatter.TraceItemReadBegin();
            this.ReadItem(reader);
            SyndicationFeedFormatter.TraceItemReadEnd();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            SyndicationFeedFormatter.TraceItemWriteBegin();
            this.WriteItem(writer);
            SyndicationFeedFormatter.TraceItemWriteEnd();
        }

        private void WriteItem(XmlWriter writer)
        {
            if (base.Item == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.ItemFormatterDoesNotHaveItem, new object[0])));
            }
            XmlDictionaryWriter dictWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            this.feedSerializer.WriteItemContents(dictWriter, base.Item);
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            SyndicationFeedFormatter.TraceItemWriteBegin();
            writer.WriteStartElement("entry", "http://www.w3.org/2005/Atom");
            this.WriteItem(writer);
            writer.WriteEndElement();
            SyndicationFeedFormatter.TraceItemWriteEnd();
        }

        protected Type ItemType =>
            this.itemType;

        public bool PreserveAttributeExtensions
        {
            get => 
                this.preserveAttributeExtensions;
            set
            {
                this.preserveAttributeExtensions = value;
                this.feedSerializer.PreserveAttributeExtensions = value;
            }
        }

        public bool PreserveElementExtensions
        {
            get => 
                this.preserveElementExtensions;
            set
            {
                this.preserveElementExtensions = value;
                this.feedSerializer.PreserveElementExtensions = value;
            }
        }

        public override string Version =>
            "Atom10";
    }
}

