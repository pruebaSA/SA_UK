namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Web.Resources;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal abstract class AbstractDataSvcMapFileLoader
    {
        private static XmlSchemaSet m_ServiceMapSchemaSet;
        protected internal XmlSerializer serializer;

        protected AbstractDataSvcMapFileLoader()
        {
        }

        protected abstract TextReader GetMapFileReader();
        public virtual void LoadExtensionFile(ExtensionFile extensionFile)
        {
            try
            {
                extensionFile.CleanUpContent();
                extensionFile.ContentBuffer = this.ReadExtensionFile(extensionFile.FileName);
            }
            catch (Exception exception)
            {
                extensionFile.ErrorInLoading = exception;
            }
        }

        public virtual DataSvcMapFile LoadMapFile(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            using (TextReader reader = this.GetMapFileReader())
            {
                List<ProxyGenerationError> proxyGenerationErrors = new List<ProxyGenerationError>();
                XmlReaderSettings settings = new XmlReaderSettings {
                    Schemas = ServiceMapSchemaSet,
                    ValidationType = ValidationType.Schema,
                    ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
                };
                ValidationEventHandler handler = delegate (object sender, ValidationEventArgs e) {
                    bool flag = e.Severity == XmlSeverityType.Error;
                    proxyGenerationErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, fileName, e.Exception, !flag));
                    if (flag)
                    {
                        throw e.Exception;
                    }
                };
                settings.ValidationEventHandler += handler;
                DataSvcMapFile svcMapFile = null;
                try
                {
                    XmlReader xmlReader = XmlReader.Create(reader, settings, (string) null);
                    try
                    {
                        svcMapFile = (DataSvcMapFile) this.Serializer.Deserialize(xmlReader);
                    }
                    catch (InvalidOperationException exception)
                    {
                        XmlException innerException = exception.InnerException as XmlException;
                        if (innerException != null)
                        {
                            throw innerException;
                        }
                        XmlSchemaException exception3 = exception.InnerException as XmlSchemaException;
                        if (exception3 == null)
                        {
                            throw;
                        }
                        if (exception3.LineNumber > 0)
                        {
                            throw new XmlSchemaException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_AppendLinePosition, new object[] { exception3.Message, exception3.LineNumber, exception3.LinePosition }), exception3, exception3.LineNumber, exception3.LinePosition);
                        }
                        throw exception3;
                    }
                    this.ValidateSvcMapFile(svcMapFile);
                    foreach (MetadataFile file2 in svcMapFile.MetadataList)
                    {
                        file2.IsExistingFile = true;
                        this.LoadMetadataFile(file2);
                    }
                    foreach (ExtensionFile file3 in svcMapFile.Extensions)
                    {
                        file3.IsExistingFile = true;
                        this.LoadExtensionFile(file3);
                    }
                }
                finally
                {
                    settings.ValidationEventHandler -= handler;
                }
                if (svcMapFile != null)
                {
                    svcMapFile.SetLoadErrors(proxyGenerationErrors);
                }
                return svcMapFile;
            }
        }

        public virtual void LoadMetadataFile(MetadataFile metadataFile)
        {
            try
            {
                metadataFile.CleanUpContent();
                metadataFile.LoadContent(this.ReadMetadataFile(metadataFile.FileName));
            }
            catch (Exception exception)
            {
                metadataFile.ErrorInLoading = exception;
            }
        }

        protected abstract byte[] ReadExtensionFile(string name);
        protected abstract byte[] ReadMetadataFile(string name);
        private void ValidateSvcMapFile(DataSvcMapFile svcMapFile)
        {
            Dictionary<string, ExternalFile> dictionary = new Dictionary<string, ExternalFile>(StringComparer.OrdinalIgnoreCase);
            foreach (MetadataFile file in svcMapFile.MetadataList)
            {
                if (file.FileName != null)
                {
                    if (dictionary.ContainsKey(file.FileName))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_TwoExternalFilesWithSameName, new object[] { file.FileName }));
                    }
                    dictionary.Add(file.FileName, file);
                }
            }
            foreach (ExtensionFile file2 in svcMapFile.Extensions)
            {
                if (file2.FileName != null)
                {
                    if (dictionary.ContainsKey(file2.FileName))
                    {
                        throw new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_TwoExternalFilesWithSameName, new object[] { file2.FileName }));
                    }
                    dictionary.Add(file2.FileName, file2);
                }
            }
        }

        protected virtual XmlSerializer Serializer
        {
            get
            {
                if (this.serializer == null)
                {
                    this.serializer = new XmlSerializer(typeof(DataSvcMapFile), "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                return this.serializer;
            }
        }

        private static XmlSchemaSet ServiceMapSchemaSet
        {
            get
            {
                if (m_ServiceMapSchemaSet == null)
                {
                    System.Xml.Schema.XmlSchema schema = System.Xml.Schema.XmlSchema.Read(typeof(AbstractDataSvcMapFileLoader).Assembly.GetManifestResourceStream(typeof(AbstractDataSvcMapFileLoader), "Schema.DataServiceMapSchema.xsd"), null);
                    m_ServiceMapSchemaSet = new XmlSchemaSet();
                    m_ServiceMapSchemaSet.Add(schema);
                }
                return m_ServiceMapSchemaSet;
            }
        }
    }
}

