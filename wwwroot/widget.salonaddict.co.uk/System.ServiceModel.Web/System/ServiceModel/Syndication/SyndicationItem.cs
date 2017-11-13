namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;

    public class SyndicationItem : IExtensibleSyndicationObject
    {
        private Collection<SyndicationPerson> authors;
        private Uri baseUri;
        private Collection<SyndicationCategory> categories;
        private SyndicationContent content;
        private Collection<SyndicationPerson> contributors;
        private TextSyndicationContent copyright;
        private ExtensibleSyndicationObject extensions;
        private string id;
        private DateTimeOffset lastUpdatedTime;
        private Collection<SyndicationLink> links;
        private DateTimeOffset publishDate;
        private SyndicationFeed sourceFeed;
        private TextSyndicationContent summary;
        private TextSyndicationContent title;

        public SyndicationItem() : this(null, null, null)
        {
        }

        protected SyndicationItem(SyndicationItem source)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (source == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.extensions = source.extensions.Clone();
            this.authors = FeedUtils.ClonePersons(source.authors);
            this.categories = FeedUtils.CloneCategories(source.categories);
            this.content = source.content?.Clone();
            this.contributors = FeedUtils.ClonePersons(source.contributors);
            this.copyright = FeedUtils.CloneTextContent(source.copyright);
            this.id = source.id;
            this.lastUpdatedTime = source.lastUpdatedTime;
            this.links = FeedUtils.CloneLinks(source.links);
            this.publishDate = source.publishDate;
            if (source.SourceFeed != null)
            {
                this.sourceFeed = source.sourceFeed.Clone(false);
                this.sourceFeed.Items = new Collection<SyndicationItem>();
            }
            this.summary = FeedUtils.CloneTextContent(source.summary);
            this.baseUri = source.baseUri;
            this.title = FeedUtils.CloneTextContent(source.title);
        }

        public SyndicationItem(string title, string content, Uri itemAlternateLink) : this(title, content, itemAlternateLink, null, DateTimeOffset.MinValue)
        {
        }

        public SyndicationItem(string title, SyndicationContent content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (title != null)
            {
                this.Title = new TextSyndicationContent(title);
            }
            this.content = content;
            if (itemAlternateLink != null)
            {
                this.Links.Add(SyndicationLink.CreateAlternateLink(itemAlternateLink));
            }
            this.id = id;
            this.lastUpdatedTime = lastUpdatedTime;
        }

        public SyndicationItem(string title, string content, Uri itemAlternateLink, string id, DateTimeOffset lastUpdatedTime) : this(title, (content != null) ? new TextSyndicationContent(content) : null, itemAlternateLink, id, lastUpdatedTime)
        {
        }

        public void AddPermalink(Uri permalink)
        {
            if (permalink == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("permalink");
            }
            this.Id = permalink.AbsoluteUri;
            this.Links.Add(SyndicationLink.CreateAlternateLink(permalink));
        }

        public virtual SyndicationItem Clone() => 
            new SyndicationItem(this);

        protected internal virtual SyndicationCategory CreateCategory() => 
            new SyndicationCategory();

        protected internal virtual SyndicationLink CreateLink() => 
            new SyndicationLink();

        protected internal virtual SyndicationPerson CreatePerson() => 
            new SyndicationPerson();

        public Atom10ItemFormatter GetAtom10Formatter() => 
            new Atom10ItemFormatter(this);

        public Rss20ItemFormatter GetRss20Formatter() => 
            this.GetRss20Formatter(true);

        public Rss20ItemFormatter GetRss20Formatter(bool serializeExtensionsAsAtom) => 
            new Rss20ItemFormatter(this, serializeExtensionsAsAtom);

        public static SyndicationItem Load(XmlReader reader) => 
            Load<SyndicationItem>(reader);

        public static TSyndicationItem Load<TSyndicationItem>(XmlReader reader) where TSyndicationItem: SyndicationItem, new()
        {
            if (reader == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            Atom10ItemFormatter<TSyndicationItem> formatter = new Atom10ItemFormatter<TSyndicationItem>();
            if (formatter.CanRead(reader))
            {
                formatter.ReadFrom(reader);
                return (formatter.Item as TSyndicationItem);
            }
            Rss20ItemFormatter<TSyndicationItem> formatter2 = new Rss20ItemFormatter<TSyndicationItem>();
            if (!formatter2.CanRead(reader))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.UnknownItemXml, new object[] { reader.LocalName, reader.NamespaceURI })));
            }
            formatter2.ReadFrom(reader);
            return (formatter2.Item as TSyndicationItem);
        }

        internal void LoadElementExtensions(XmlBuffer buffer)
        {
            this.extensions.LoadElementExtensions(buffer);
        }

        internal void LoadElementExtensions(XmlReader readerOverUnparsedExtensions, int maxExtensionSize)
        {
            this.extensions.LoadElementExtensions(readerOverUnparsedExtensions, maxExtensionSize);
        }

        public void SaveAsAtom10(XmlWriter writer)
        {
            this.GetAtom10Formatter().WriteTo(writer);
        }

        public void SaveAsRss20(XmlWriter writer)
        {
            this.GetRss20Formatter().WriteTo(writer);
        }

        protected internal virtual bool TryParseAttribute(string name, string ns, string value, string version) => 
            false;

        protected internal virtual bool TryParseContent(XmlReader reader, string contentType, string version, out SyndicationContent content)
        {
            content = null;
            return false;
        }

        protected internal virtual bool TryParseElement(XmlReader reader, string version) => 
            false;

        protected internal virtual void WriteAttributeExtensions(XmlWriter writer, string version)
        {
            this.extensions.WriteAttributeExtensions(writer);
        }

        protected internal virtual void WriteElementExtensions(XmlWriter writer, string version)
        {
            this.extensions.WriteElementExtensions(writer);
        }

        public Dictionary<XmlQualifiedName, string> AttributeExtensions =>
            this.extensions.AttributeExtensions;

        public Collection<SyndicationPerson> Authors
        {
            get
            {
                if (this.authors == null)
                {
                    this.authors = new NullNotAllowedCollection<SyndicationPerson>();
                }
                return this.authors;
            }
        }

        public Uri BaseUri
        {
            get => 
                this.baseUri;
            set
            {
                this.baseUri = value;
            }
        }

        public Collection<SyndicationCategory> Categories
        {
            get
            {
                if (this.categories == null)
                {
                    this.categories = new NullNotAllowedCollection<SyndicationCategory>();
                }
                return this.categories;
            }
        }

        public SyndicationContent Content
        {
            get => 
                this.content;
            set
            {
                this.content = value;
            }
        }

        public Collection<SyndicationPerson> Contributors
        {
            get
            {
                if (this.contributors == null)
                {
                    this.contributors = new NullNotAllowedCollection<SyndicationPerson>();
                }
                return this.contributors;
            }
        }

        public TextSyndicationContent Copyright
        {
            get => 
                this.copyright;
            set
            {
                this.copyright = value;
            }
        }

        public SyndicationElementExtensionCollection ElementExtensions =>
            this.extensions.ElementExtensions;

        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        public DateTimeOffset LastUpdatedTime
        {
            get => 
                this.lastUpdatedTime;
            set
            {
                this.lastUpdatedTime = value;
            }
        }

        public Collection<SyndicationLink> Links
        {
            get
            {
                if (this.links == null)
                {
                    this.links = new NullNotAllowedCollection<SyndicationLink>();
                }
                return this.links;
            }
        }

        public DateTimeOffset PublishDate
        {
            get => 
                this.publishDate;
            set
            {
                this.publishDate = value;
            }
        }

        public SyndicationFeed SourceFeed
        {
            get => 
                this.sourceFeed;
            set
            {
                this.sourceFeed = value;
            }
        }

        public TextSyndicationContent Summary
        {
            get => 
                this.summary;
            set
            {
                this.summary = value;
            }
        }

        public TextSyndicationContent Title
        {
            get => 
                this.title;
            set
            {
                this.title = value;
            }
        }
    }
}

