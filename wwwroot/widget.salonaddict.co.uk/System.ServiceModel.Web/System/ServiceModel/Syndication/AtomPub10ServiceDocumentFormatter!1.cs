namespace System.ServiceModel.Syndication
{
    using System;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="service", Namespace="http://www.w3.org/2007/app")]
    public class AtomPub10ServiceDocumentFormatter<TServiceDocument> : AtomPub10ServiceDocumentFormatter where TServiceDocument: ServiceDocument, new()
    {
        public AtomPub10ServiceDocumentFormatter() : base(typeof(TServiceDocument))
        {
        }

        public AtomPub10ServiceDocumentFormatter(TServiceDocument documentToWrite) : base(documentToWrite)
        {
        }

        protected override ServiceDocument CreateDocumentInstance() => 
            Activator.CreateInstance<TServiceDocument>();
    }
}

