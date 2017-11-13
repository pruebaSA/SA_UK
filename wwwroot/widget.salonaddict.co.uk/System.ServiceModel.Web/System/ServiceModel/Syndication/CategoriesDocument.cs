namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.Xml;

    public abstract class CategoriesDocument : IExtensibleSyndicationObject
    {
        private Uri baseUri;
        private ExtensibleSyndicationObject extensions = new ExtensibleSyndicationObject();
        private string language;

        internal CategoriesDocument()
        {
        }

        public static InlineCategoriesDocument Create(Collection<SyndicationCategory> categories) => 
            new InlineCategoriesDocument(categories);

        public static ReferencedCategoriesDocument Create(Uri linkToCategoriesDocument) => 
            new ReferencedCategoriesDocument(linkToCategoriesDocument);

        public static InlineCategoriesDocument Create(Collection<SyndicationCategory> categories, bool isFixed, string scheme) => 
            new InlineCategoriesDocument(categories, isFixed, scheme);

        public CategoriesDocumentFormatter GetFormatter() => 
            new AtomPub10CategoriesDocumentFormatter(this);

        public static CategoriesDocument Load(XmlReader reader)
        {
            AtomPub10CategoriesDocumentFormatter formatter = new AtomPub10CategoriesDocumentFormatter();
            formatter.ReadFrom(reader);
            return formatter.Document;
        }

        internal void LoadElementExtensions(XmlBuffer buffer)
        {
            this.extensions.LoadElementExtensions(buffer);
        }

        internal void LoadElementExtensions(XmlReader readerOverUnparsedExtensions, int maxExtensionSize)
        {
            this.extensions.LoadElementExtensions(readerOverUnparsedExtensions, maxExtensionSize);
        }

        public void Save(XmlWriter writer)
        {
            this.GetFormatter().WriteTo(writer);
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

        public SyndicationElementExtensionCollection ElementExtensions =>
            this.extensions.ElementExtensions;

        internal abstract bool IsInline { get; }

        public string Language
        {
            get => 
                this.language;
            set
            {
                this.language = value;
            }
        }
    }
}

