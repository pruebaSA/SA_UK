namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.Xml;

    public class ServiceDocument : IExtensibleSyndicationObject
    {
        private Uri baseUri;
        private ExtensibleSyndicationObject extensions;
        private string language;
        private Collection<Workspace> workspaces;

        public ServiceDocument() : this(null)
        {
        }

        public ServiceDocument(IEnumerable<Workspace> workspaces)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (workspaces != null)
            {
                this.workspaces = new NullNotAllowedCollection<Workspace>();
                foreach (Workspace workspace in workspaces)
                {
                    this.workspaces.Add(workspace);
                }
            }
        }

        protected internal virtual Workspace CreateWorkspace() => 
            new Workspace();

        public ServiceDocumentFormatter GetFormatter() => 
            new AtomPub10ServiceDocumentFormatter(this);

        public static ServiceDocument Load(XmlReader reader) => 
            Load<ServiceDocument>(reader);

        public static TServiceDocument Load<TServiceDocument>(XmlReader reader) where TServiceDocument: ServiceDocument, new()
        {
            AtomPub10ServiceDocumentFormatter<TServiceDocument> formatter = new AtomPub10ServiceDocumentFormatter<TServiceDocument>();
            formatter.ReadFrom(reader);
            return (TServiceDocument) formatter.Document;
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
            new AtomPub10ServiceDocumentFormatter(this).WriteTo(writer);
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

        public string Language
        {
            get => 
                this.language;
            set
            {
                this.language = value;
            }
        }

        public Collection<Workspace> Workspaces
        {
            get
            {
                if (this.workspaces == null)
                {
                    this.workspaces = new NullNotAllowedCollection<Workspace>();
                }
                return this.workspaces;
            }
        }
    }
}

