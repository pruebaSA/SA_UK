namespace System.ServiceModel.Syndication
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="entry", Namespace="http://www.w3.org/2005/Atom")]
    public class Atom10ItemFormatter<TSyndicationItem> : Atom10ItemFormatter where TSyndicationItem: SyndicationItem, new()
    {
        public Atom10ItemFormatter() : base(typeof(TSyndicationItem))
        {
        }

        public Atom10ItemFormatter(TSyndicationItem itemToWrite) : base(itemToWrite)
        {
        }

        protected override SyndicationItem CreateItemInstance() => 
            Activator.CreateInstance<TSyndicationItem>();
    }
}

