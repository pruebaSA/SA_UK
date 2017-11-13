namespace System.ServiceModel.Syndication
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [XmlRoot(ElementName="categories", Namespace="http://www.w3.org/2007/app")]
    public class AtomPub10CategoriesDocumentFormatter : CategoriesDocumentFormatter, IXmlSerializable
    {
        private Type inlineDocumentType;
        private int maxExtensionSize;
        private bool preserveAttributeExtensions;
        private bool preserveElementExtensions;
        private Type referencedDocumentType;

        public AtomPub10CategoriesDocumentFormatter() : this(typeof(InlineCategoriesDocument), typeof(ReferencedCategoriesDocument))
        {
        }

        public AtomPub10CategoriesDocumentFormatter(CategoriesDocument documentToWrite) : base(documentToWrite)
        {
            this.maxExtensionSize = 0x7fffffff;
            this.preserveAttributeExtensions = true;
            this.preserveElementExtensions = true;
            if (documentToWrite.IsInline)
            {
                this.inlineDocumentType = documentToWrite.GetType();
                this.referencedDocumentType = typeof(ReferencedCategoriesDocument);
            }
            else
            {
                this.referencedDocumentType = documentToWrite.GetType();
                this.inlineDocumentType = typeof(InlineCategoriesDocument);
            }
        }

        public AtomPub10CategoriesDocumentFormatter(Type inlineDocumentType, Type referencedDocumentType)
        {
            if (inlineDocumentType == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("inlineDocumentType");
            }
            if (!typeof(InlineCategoriesDocument).IsAssignableFrom(inlineDocumentType))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("inlineDocumentType", SR2.GetString(SR2.InvalidObjectTypePassed, new object[] { "inlineDocumentType", "InlineCategoriesDocument" }));
            }
            if (referencedDocumentType == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("referencedDocumentType");
            }
            if (!typeof(ReferencedCategoriesDocument).IsAssignableFrom(referencedDocumentType))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("referencedDocumentType", SR2.GetString(SR2.InvalidObjectTypePassed, new object[] { "referencedDocumentType", "ReferencedCategoriesDocument" }));
            }
            this.maxExtensionSize = 0x7fffffff;
            this.preserveAttributeExtensions = true;
            this.preserveElementExtensions = true;
            this.inlineDocumentType = inlineDocumentType;
            this.referencedDocumentType = referencedDocumentType;
        }

        public override bool CanRead(XmlReader reader) => 
            reader?.IsStartElement("categories", "http://www.w3.org/2007/app");

        protected override InlineCategoriesDocument CreateInlineCategoriesDocument()
        {
            if (this.inlineDocumentType == typeof(InlineCategoriesDocument))
            {
                return new InlineCategoriesDocument();
            }
            return (InlineCategoriesDocument) Activator.CreateInstance(this.inlineDocumentType);
        }

        protected override ReferencedCategoriesDocument CreateReferencedCategoriesDocument()
        {
            if (this.referencedDocumentType == typeof(ReferencedCategoriesDocument))
            {
                return new ReferencedCategoriesDocument();
            }
            return (ReferencedCategoriesDocument) Activator.CreateInstance(this.referencedDocumentType);
        }

        private void ReadDocument(XmlReader reader)
        {
            CreateInlineCategoriesDelegate inlineCategoriesFactory = null;
            CreateReferencedCategoriesDelegate referencedCategoriesFactory = null;
            try
            {
                SyndicationFeedFormatter.MoveToStartElement(reader);
                if (inlineCategoriesFactory == null)
                {
                    inlineCategoriesFactory = () => this.CreateInlineCategoriesDocument();
                }
                if (referencedCategoriesFactory == null)
                {
                    referencedCategoriesFactory = () => this.CreateReferencedCategoriesDocument();
                }
                this.SetDocument(AtomPub10ServiceDocumentFormatter.ReadCategories(reader, null, inlineCategoriesFactory, referencedCategoriesFactory, this.Version, this.preserveElementExtensions, this.preserveAttributeExtensions, this.maxExtensionSize));
            }
            catch (FormatException exception)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(FeedUtils.AddLineInfo(reader, SR2.ErrorParsingDocument), exception));
            }
            catch (ArgumentException exception2)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(FeedUtils.AddLineInfo(reader, SR2.ErrorParsingDocument), exception2));
            }
        }

        public override void ReadFrom(XmlReader reader)
        {
            if (reader == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            if (!this.CanRead(reader))
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.UnknownDocumentXml, new object[] { reader.LocalName, reader.NamespaceURI })));
            }
            TraceCategoriesDocumentReadBegin();
            this.ReadDocument(reader);
            TraceCategoriesDocumentReadEnd();
        }

        XmlSchema IXmlSerializable.GetSchema() => 
            null;

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            TraceCategoriesDocumentReadBegin();
            this.ReadDocument(reader);
            TraceCategoriesDocumentReadEnd();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            if (base.Document == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.DocumentFormatterDoesNotHaveDocument, new object[0])));
            }
            TraceCategoriesDocumentWriteBegin();
            this.WriteDocument(writer);
            TraceCategoriesDocumentWriteEnd();
        }

        internal static void TraceCategoriesDocumentReadBegin()
        {
            if (System.Runtime.Serialization.DiagnosticUtility.ShouldTraceInformation)
            {
                System.Runtime.Serialization.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadCategoriesDocumentBegin, SR2.GetString(SR2.TraceCodeSyndicationReadCategoriesDocumentBegin, new object[0]));
            }
        }

        internal static void TraceCategoriesDocumentReadEnd()
        {
            if (System.Runtime.Serialization.DiagnosticUtility.ShouldTraceInformation)
            {
                System.Runtime.Serialization.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationReadCategoriesDocumentEnd, SR2.GetString(SR2.TraceCodeSyndicationReadCategoriesDocumentEnd, new object[0]));
            }
        }

        internal static void TraceCategoriesDocumentWriteBegin()
        {
            if (System.Runtime.Serialization.DiagnosticUtility.ShouldTraceInformation)
            {
                System.Runtime.Serialization.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteCategoriesDocumentBegin, SR2.GetString(SR2.TraceCodeSyndicationWriteCategoriesDocumentBegin, new object[0]));
            }
        }

        internal static void TraceCategoriesDocumentWriteEnd()
        {
            if (System.Runtime.Serialization.DiagnosticUtility.ShouldTraceInformation)
            {
                System.Runtime.Serialization.DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SyndicationWriteCategoriesDocumentEnd, SR2.GetString(SR2.TraceCodeSyndicationWriteCategoriesDocumentEnd, new object[0]));
            }
        }

        private void WriteDocument(XmlWriter writer)
        {
            writer.WriteAttributeString("a10", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2005/Atom");
            AtomPub10ServiceDocumentFormatter.WriteCategoriesInnerXml(writer, base.Document, null, this.Version);
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            if (base.Document == null)
            {
                throw System.Runtime.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.DocumentFormatterDoesNotHaveDocument, new object[0])));
            }
            TraceCategoriesDocumentWriteBegin();
            writer.WriteStartElement("app", "categories", "http://www.w3.org/2007/app");
            this.WriteDocument(writer);
            writer.WriteEndElement();
            TraceCategoriesDocumentWriteEnd();
        }

        public override string Version =>
            "http://www.w3.org/2007/app";
    }
}

