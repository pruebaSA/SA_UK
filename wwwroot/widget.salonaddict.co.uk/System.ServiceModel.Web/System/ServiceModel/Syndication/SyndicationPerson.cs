namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;

    public class SyndicationPerson : IExtensibleSyndicationObject
    {
        private string email;
        private ExtensibleSyndicationObject extensions;
        private string name;
        private string uri;

        public SyndicationPerson() : this((string) null)
        {
        }

        protected SyndicationPerson(SyndicationPerson source)
        {
            this.extensions = new ExtensibleSyndicationObject();
            if (source == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.email = source.email;
            this.name = source.name;
            this.uri = source.uri;
            this.extensions = source.extensions.Clone();
        }

        public SyndicationPerson(string email) : this(email, null, null)
        {
        }

        public SyndicationPerson(string email, string name, string uri)
        {
            this.extensions = new ExtensibleSyndicationObject();
            this.name = name;
            this.email = email;
            this.uri = uri;
        }

        public virtual SyndicationPerson Clone() => 
            new SyndicationPerson(this);

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

        public SyndicationElementExtensionCollection ElementExtensions =>
            this.extensions.ElementExtensions;

        public string Email
        {
            get => 
                this.email;
            set
            {
                this.email = value;
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public string Uri
        {
            get => 
                this.uri;
            set
            {
                this.uri = value;
            }
        }
    }
}

