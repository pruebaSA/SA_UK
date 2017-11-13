namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;

    public class SyndicationFeed : IExtensibleSyndicationObject
    {
        private Collection<SyndicationPerson> authors;
        private Uri baseUri;
        private Collection<SyndicationCategory> categories;
        private Collection<SyndicationPerson> contributors;
        private TextSyndicationContent copyright;
        private TextSyndicationContent description;
        private ExtensibleSyndicationObject extensions;
        private string generator;
        private string id;
        private Uri imageUrl;
        private IEnumerable<SyndicationItem> items;
        private string language;
        private DateTimeOffset lastUpdatedTime;
        private Collection<SyndicationLink> links;
        private TextSyndicationContent title;

        public SyndicationFeed() : this(null)
        {
        }

        public SyndicationFeed(IEnumerable<SyndicationItem> items) : this(null, null, null, items)
        {
        }

        protected SyndicationFeed(SyndicationFeed source, bool cloneItems)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (source == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.authors = FeedUtils.ClonePersons(source.authors);
            this.categories = FeedUtils.CloneCategories(source.categories);
            this.contributors = FeedUtils.ClonePersons(source.contributors);
            this.copyright = FeedUtils.CloneTextContent(source.copyright);
            this.description = FeedUtils.CloneTextContent(source.description);
            this.extensions = source.extensions.Clone();
            this.generator = source.generator;
            this.id = source.id;
            this.imageUrl = source.imageUrl;
            this.language = source.language;
            this.lastUpdatedTime = source.lastUpdatedTime;
            this.links = FeedUtils.CloneLinks(source.links);
            this.title = FeedUtils.CloneTextContent(source.title);
            this.baseUri = source.baseUri;
            IList<SyndicationItem> items = source.items as IList<SyndicationItem>;
            if (items != null)
            {
                Collection<SyndicationItem> collection = new NullNotAllowedCollection<SyndicationItem>();
                for (int i = 0; i < items.Count; i++)
                {
                    collection.Add(cloneItems ? items[i].Clone() : items[i]);
                }
                this.items = collection;
            }
            else
            {
                if (cloneItems)
                {
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UnbufferedItemsCannotBeCloned, new object[0])));
                }
                this.items = source.items;
            }
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink) : this(title, description, feedAlternateLink, null)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, IEnumerable<SyndicationItem> items) : this(title, description, feedAlternateLink, null, DateTimeOffset.MinValue, items)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, string id, DateTimeOffset lastUpdatedTime) : this(title, description, feedAlternateLink, id, lastUpdatedTime, null)
        {
        }

        public SyndicationFeed(string title, string description, Uri feedAlternateLink, string id, DateTimeOffset lastUpdatedTime, IEnumerable<SyndicationItem> items)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (title != null)
            {
                this.title = new TextSyndicationContent(title);
            }
            if (description != null)
            {
                this.description = new TextSyndicationContent(description);
            }
            if (feedAlternateLink != null)
            {
                this.Links.Add(SyndicationLink.CreateAlternateLink(feedAlternateLink));
            }
            this.id = id;
            this.lastUpdatedTime = lastUpdatedTime;
            this.items = items;
        }

        public virtual SyndicationFeed Clone(bool cloneItems) => 
            new SyndicationFeed(this, cloneItems);

        protected internal virtual SyndicationCategory CreateCategory() => 
            new SyndicationCategory();

        protected internal virtual SyndicationItem CreateItem() => 
            new SyndicationItem();

        protected internal virtual SyndicationLink CreateLink() => 
            new SyndicationLink();

        protected internal virtual SyndicationPerson CreatePerson() => 
            new SyndicationPerson();

        public Atom10FeedFormatter GetAtom10Formatter() => 
            new Atom10FeedFormatter(this);

        public Rss20FeedFormatter GetRss20Formatter() => 
            this.GetRss20Formatter(true);

        public Rss20FeedFormatter GetRss20Formatter(bool serializeExtensionsAsAtom) => 
            new Rss20FeedFormatter(this, serializeExtensionsAsAtom);

        public static SyndicationFeed Load(XmlReader reader) => 
            Load<SyndicationFeed>(reader);

        public static TSyndicationFeed Load<TSyndicationFeed>(XmlReader reader) where TSyndicationFeed: SyndicationFeed, new()
        {
            if (reader == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            Atom10FeedFormatter<TSyndicationFeed> formatter = new Atom10FeedFormatter<TSyndicationFeed>();
            if (formatter.CanRead(reader))
            {
                formatter.ReadFrom(reader);
                return (formatter.Feed as TSyndicationFeed);
            }
            Rss20FeedFormatter<TSyndicationFeed> formatter2 = new Rss20FeedFormatter<TSyndicationFeed>();
            if (!formatter2.CanRead(reader))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.UnknownFeedXml, new object[] { reader.LocalName, reader.NamespaceURI })));
            }
            formatter2.ReadFrom(reader);
            return (formatter2.Feed as TSyndicationFeed);
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

        public TextSyndicationContent Description
        {
            get => 
                this.description;
            set
            {
                this.description = value;
            }
        }

        public SyndicationElementExtensionCollection ElementExtensions =>
            this.extensions.ElementExtensions;

        public string Generator
        {
            get => 
                this.generator;
            set
            {
                this.generator = value;
            }
        }

        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        public Uri ImageUrl
        {
            get => 
                this.imageUrl;
            set
            {
                this.imageUrl = value;
            }
        }

        public IEnumerable<SyndicationItem> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new NullNotAllowedCollection<SyndicationItem>();
                }
                return this.items;
            }
            set
            {
                if (value == null)
                {
                    throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.items = value;
            }
        }

        public string Language
        {
            get => 
                this.language;
            set
            {
                this.language = value;
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

