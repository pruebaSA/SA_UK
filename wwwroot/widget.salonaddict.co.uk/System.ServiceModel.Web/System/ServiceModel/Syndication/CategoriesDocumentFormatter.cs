namespace System.ServiceModel.Syndication
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;

    [DataContract]
    public abstract class CategoriesDocumentFormatter
    {
        private CategoriesDocument document;

        protected CategoriesDocumentFormatter()
        {
        }

        protected CategoriesDocumentFormatter(CategoriesDocument documentToWrite)
        {
            if (documentToWrite == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("documentToWrite");
            }
            this.document = documentToWrite;
        }

        public abstract bool CanRead(XmlReader reader);
        protected virtual InlineCategoriesDocument CreateInlineCategoriesDocument() => 
            new InlineCategoriesDocument();

        protected virtual ReferencedCategoriesDocument CreateReferencedCategoriesDocument() => 
            new ReferencedCategoriesDocument();

        public abstract void ReadFrom(XmlReader reader);
        protected virtual void SetDocument(CategoriesDocument document)
        {
            this.document = document;
        }

        public abstract void WriteTo(XmlWriter writer);

        public CategoriesDocument Document =>
            this.document;

        public abstract string Version { get; }
    }
}

