namespace System.ServiceModel.Syndication
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class SyndicationContent
    {
        private Dictionary<XmlQualifiedName, string> attributeExtensions;

        protected SyndicationContent()
        {
        }

        protected SyndicationContent(SyndicationContent source)
        {
            this.CopyAttributeExtensions(source);
        }

        public abstract SyndicationContent Clone();
        internal void CopyAttributeExtensions(SyndicationContent source)
        {
            if (source == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            if (source.attributeExtensions != null)
            {
                foreach (XmlQualifiedName name in source.attributeExtensions.Keys)
                {
                    this.AttributeExtensions.Add(name, source.attributeExtensions[name]);
                }
            }
        }

        public static TextSyndicationContent CreateHtmlContent(string content) => 
            new TextSyndicationContent(content, TextSyndicationContentKind.Html);

        public static TextSyndicationContent CreatePlaintextContent(string content) => 
            new TextSyndicationContent(content);

        public static UrlSyndicationContent CreateUrlContent(Uri url, string mediaType) => 
            new UrlSyndicationContent(url, mediaType);

        public static TextSyndicationContent CreateXhtmlContent(string content) => 
            new TextSyndicationContent(content, TextSyndicationContentKind.XHtml);

        public static XmlSyndicationContent CreateXmlContent(object dataContractObject) => 
            new XmlSyndicationContent("text/xml", dataContractObject, null);

        public static XmlSyndicationContent CreateXmlContent(XmlReader xmlReader) => 
            new XmlSyndicationContent(xmlReader);

        public static XmlSyndicationContent CreateXmlContent(object dataContractObject, XmlObjectSerializer dataContractSerializer) => 
            new XmlSyndicationContent("text/xml", dataContractObject, dataContractSerializer);

        public static XmlSyndicationContent CreateXmlContent(object xmlSerializerObject, XmlSerializer serializer) => 
            new XmlSyndicationContent("text/xml", xmlSerializerObject, serializer);

        protected abstract void WriteContentsTo(XmlWriter writer);
        public void WriteTo(XmlWriter writer, string outerElementName, string outerElementNamespace)
        {
            if (writer == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            if (string.IsNullOrEmpty(outerElementName))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR2.GetString(SR2.OuterElementNameNotSpecified, new object[0]));
            }
            writer.WriteStartElement(outerElementName, outerElementNamespace);
            writer.WriteAttributeString("type", string.Empty, this.Type);
            if (this.attributeExtensions != null)
            {
                foreach (XmlQualifiedName name in this.attributeExtensions.Keys)
                {
                    string str;
                    if (((name.Name != "type") || (name.Namespace != string.Empty)) && this.attributeExtensions.TryGetValue(name, out str))
                    {
                        writer.WriteAttributeString(name.Name, name.Namespace, str);
                    }
                }
            }
            this.WriteContentsTo(writer);
            writer.WriteEndElement();
        }

        public Dictionary<XmlQualifiedName, string> AttributeExtensions
        {
            get
            {
                if (this.attributeExtensions == null)
                {
                    this.attributeExtensions = new Dictionary<XmlQualifiedName, string>();
                }
                return this.attributeExtensions;
            }
        }

        public abstract string Type { get; }
    }
}

