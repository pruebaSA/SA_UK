namespace System.ServiceModel.Syndication
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="rss", Namespace="")]
    public class Rss20FeedFormatter<TSyndicationFeed> : Rss20FeedFormatter where TSyndicationFeed: SyndicationFeed, new()
    {
        public Rss20FeedFormatter() : base(typeof(TSyndicationFeed))
        {
        }

        public Rss20FeedFormatter(TSyndicationFeed feedToWrite) : base(feedToWrite)
        {
        }

        public Rss20FeedFormatter(TSyndicationFeed feedToWrite, bool serializeExtensionsAsAtom) : base(feedToWrite, serializeExtensionsAsAtom)
        {
        }

        protected override SyndicationFeed CreateFeedInstance() => 
            Activator.CreateInstance<TSyndicationFeed>();
    }
}

