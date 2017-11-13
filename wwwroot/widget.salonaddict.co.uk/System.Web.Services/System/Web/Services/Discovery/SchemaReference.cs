namespace System.Web.Services.Discovery
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Web.Services;
    using System.Web.Services.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [XmlRoot("schemaRef", Namespace="http://schemas.xmlsoap.org/disco/schema/")]
    public sealed class SchemaReference : DiscoveryReference
    {
        public const string Namespace = "http://schemas.xmlsoap.org/disco/schema/";
        private string reference;
        private string targetNamespace;

        public SchemaReference()
        {
        }

        public SchemaReference(string url)
        {
            this.Ref = url;
        }

        internal XmlSchema GetSchema()
        {
            try
            {
                return this.Schema;
            }
            catch (Exception exception)
            {
                if (((exception is ThreadAbortException) || (exception is StackOverflowException)) || (exception is OutOfMemoryException))
                {
                    throw;
                }
                base.ClientProtocol.Errors[this.Url] = exception;
                if (Tracing.On)
                {
                    Tracing.ExceptionCatch(TraceEventType.Warning, this, "GetSchema", exception);
                }
            }
            catch
            {
                base.ClientProtocol.Errors[this.Url] = new Exception(System.Web.Services.Res.GetString("NonClsCompliantException"));
            }
            return null;
        }

        internal override void LoadExternals(Hashtable loadedExternals)
        {
            LoadExternals(this.GetSchema(), this.Url, base.ClientProtocol, loadedExternals);
        }

        internal static void LoadExternals(XmlSchema schema, string url, DiscoveryClientProtocol client, Hashtable loadedExternals)
        {
            if (schema != null)
            {
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    if ((((external.SchemaLocation != null) && (external.SchemaLocation.Length != 0)) && (external.Schema == null)) && ((external is XmlSchemaInclude) || (external is XmlSchemaRedefine)))
                    {
                        string str = DiscoveryReference.UriToString(url, external.SchemaLocation);
                        if (client.References[str] is SchemaReference)
                        {
                            SchemaReference reference = (SchemaReference) client.References[str];
                            external.Schema = reference.GetSchema();
                            if (external.Schema != null)
                            {
                                loadedExternals[str] = external.Schema;
                            }
                            reference.LoadExternals(loadedExternals);
                        }
                    }
                }
            }
        }

        public override object ReadDocument(Stream stream)
        {
            XmlTextReader reader = new XmlTextReader(this.Url, stream) {
                XmlResolver = null
            };
            return XmlSchema.Read(reader, null);
        }

        protected internal override void Resolve(string contentType, Stream stream)
        {
            if (ContentType.IsHtml(contentType))
            {
                base.ClientProtocol.Errors[this.Url] = new InvalidContentTypeException(System.Web.Services.Res.GetString("WebInvalidContentType", new object[] { contentType }), contentType);
            }
            XmlSchema schema = base.ClientProtocol.Documents[this.Url] as XmlSchema;
            if (schema == null)
            {
                if (base.ClientProtocol.Errors[this.Url] != null)
                {
                    throw base.ClientProtocol.Errors[this.Url];
                }
                schema = (XmlSchema) this.ReadDocument(stream);
                base.ClientProtocol.Documents[this.Url] = schema;
            }
            if (base.ClientProtocol.References[this.Url] != this)
            {
                base.ClientProtocol.References[this.Url] = this;
            }
            foreach (XmlSchemaExternal external in schema.Includes)
            {
                string url = null;
                try
                {
                    if ((external.SchemaLocation != null) && (external.SchemaLocation.Length > 0))
                    {
                        url = DiscoveryReference.UriToString(this.Url, external.SchemaLocation);
                        SchemaReference reference = new SchemaReference(url) {
                            ClientProtocol = base.ClientProtocol
                        };
                        base.ClientProtocol.References[url] = reference;
                        reference.Resolve();
                    }
                }
                catch (Exception exception)
                {
                    if (((exception is ThreadAbortException) || (exception is StackOverflowException)) || (exception is OutOfMemoryException))
                    {
                        throw;
                    }
                    throw new InvalidDocumentContentsException(System.Web.Services.Res.GetString("TheSchemaDocumentContainsLinksThatCouldNotBeResolved", new object[] { url }), exception);
                }
                catch
                {
                    throw new InvalidDocumentContentsException(System.Web.Services.Res.GetString("TheSchemaDocumentContainsLinksThatCouldNotBeResolved", new object[] { url }), null);
                }
            }
        }

        public override void WriteDocument(object document, Stream stream)
        {
            ((XmlSchema) document).Write(new StreamWriter(stream, new UTF8Encoding(false)));
        }

        [XmlIgnore]
        public override string DefaultFilename
        {
            get
            {
                string path = DiscoveryReference.MakeValidFilename(this.Schema.Id);
                if ((path == null) || (path.Length == 0))
                {
                    path = DiscoveryReference.FilenameFromUrl(this.Url);
                }
                return Path.ChangeExtension(path, ".xsd");
            }
        }

        [XmlAttribute("ref")]
        public string Ref
        {
            get
            {
                if (this.reference != null)
                {
                    return this.reference;
                }
                return "";
            }
            set
            {
                this.reference = value;
            }
        }

        [XmlIgnore]
        public XmlSchema Schema
        {
            get
            {
                if (base.ClientProtocol == null)
                {
                    throw new InvalidOperationException(System.Web.Services.Res.GetString("WebMissingClientProtocol"));
                }
                object obj2 = base.ClientProtocol.InlinedSchemas[this.Url];
                if (obj2 == null)
                {
                    obj2 = base.ClientProtocol.Documents[this.Url];
                }
                if (obj2 == null)
                {
                    base.Resolve();
                    obj2 = base.ClientProtocol.Documents[this.Url];
                }
                XmlSchema schema = obj2 as XmlSchema;
                if (schema == null)
                {
                    throw new InvalidOperationException(System.Web.Services.Res.GetString("WebInvalidDocType", new object[] { typeof(XmlSchema).FullName, (obj2 == null) ? string.Empty : obj2.GetType().FullName, this.Url }));
                }
                return schema;
            }
        }

        [DefaultValue((string) null), XmlAttribute("targetNamespace")]
        public string TargetNamespace
        {
            get => 
                this.targetNamespace;
            set
            {
                this.targetNamespace = value;
            }
        }

        [XmlIgnore]
        public override string Url
        {
            get => 
                this.Ref;
            set
            {
                this.Ref = value;
            }
        }
    }
}

