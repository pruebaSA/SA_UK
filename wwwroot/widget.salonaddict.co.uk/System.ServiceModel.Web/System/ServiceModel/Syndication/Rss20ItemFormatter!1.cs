namespace System.ServiceModel.Syndication
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="item", Namespace="")]
    public class Rss20ItemFormatter<TSyndicationItem> : Rss20ItemFormatter, IXmlSerializable where TSyndicationItem: SyndicationItem, new()
    {
        public Rss20ItemFormatter() : base(typeof(TSyndicationItem))
        {
        }

        public Rss20ItemFormatter(TSyndicationItem itemToWrite) : base(itemToWrite)
        {
        }

        public Rss20ItemFormatter(TSyndicationItem itemToWrite, bool serializeExtensionsAsAtom) : base(itemToWrite, serializeExtensionsAsAtom)
        {
        }

        protected override SyndicationItem CreateItemInstance() => 
            Activator.CreateInstance<TSyndicationItem>();
    }
}

