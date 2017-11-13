﻿namespace System.ServiceModel.Syndication
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="feed", Namespace="http://www.w3.org/2005/Atom")]
    public class Atom10FeedFormatter<TSyndicationFeed> : Atom10FeedFormatter where TSyndicationFeed: SyndicationFeed, new()
    {
        public Atom10FeedFormatter() : base(typeof(TSyndicationFeed))
        {
        }

        public Atom10FeedFormatter(TSyndicationFeed feedToWrite) : base(feedToWrite)
        {
        }

        protected override SyndicationFeed CreateFeedInstance() => 
            Activator.CreateInstance<TSyndicationFeed>();
    }
}

