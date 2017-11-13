namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.Xml;

    public class Workspace : IExtensibleSyndicationObject
    {
        private Uri baseUri;
        private Collection<ResourceCollectionInfo> collections;
        private ExtensibleSyndicationObject extensions;
        private TextSyndicationContent title;

        public Workspace()
        {
            this.extensions = new ExtensibleSyndicationObject();
        }

        public Workspace(TextSyndicationContent title, IEnumerable<ResourceCollectionInfo> collections)
        {
            this.extensions = new ExtensibleSyndicationObject();
            this.title = title;
            if (collections != null)
            {
                this.collections = new NullNotAllowedCollection<ResourceCollectionInfo>();
                foreach (ResourceCollectionInfo info in collections)
                {
                    this.collections.Add(info);
                }
            }
        }

        public Workspace(string title, IEnumerable<ResourceCollectionInfo> collections) : this((title != null) ? new TextSyndicationContent(title) : null, collections)
        {
        }

        protected internal virtual ResourceCollectionInfo CreateResourceCollection() => 
            new ResourceCollectionInfo();

        internal void LoadElementExtensions(XmlBuffer buffer)
        {
            this.extensions.LoadElementExtensions(buffer);
        }

        internal void LoadElementExtensions(XmlReader readerOverUnparsedExtensions, int maxExtensionSize)
        {
            this.extensions.LoadElementExtensions(readerOverUnparsedExtensions, maxExtensionSize);
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

        public Uri BaseUri
        {
            get => 
                this.baseUri;
            set
            {
                this.baseUri = value;
            }
        }

        public Collection<ResourceCollectionInfo> Collections
        {
            get
            {
                if (this.collections == null)
                {
                    this.collections = new NullNotAllowedCollection<ResourceCollectionInfo>();
                }
                return this.collections;
            }
        }

        public SyndicationElementExtensionCollection ElementExtensions =>
            this.extensions.ElementExtensions;

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

