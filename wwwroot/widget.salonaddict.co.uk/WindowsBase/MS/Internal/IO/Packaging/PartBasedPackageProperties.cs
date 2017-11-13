namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Text;
    using System.Windows;
    using System.Xml;

    internal class PartBasedPackageProperties : PackageProperties
    {
        private static readonly MS.Internal.ContentType _coreDocumentPropertiesContentType = new MS.Internal.ContentType("application/vnd.openxmlformats-package.core-properties+xml");
        private const string _coreDocumentPropertiesRelationshipType = "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties";
        private static readonly string[] _dateTimeFormats = new string[] { 
            "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:sszzz", @"\-yyyy-MM-ddTHH:mm:ss", @"\-yyyy-MM-ddTHH:mm:ssZ", @"\-yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss.ff", "yyyy-MM-ddTHH:mm:ss.fZ", "yyyy-MM-ddTHH:mm:ss.fzzz", @"\-yyyy-MM-ddTHH:mm:ss.f", @"\-yyyy-MM-ddTHH:mm:ss.fZ", @"\-yyyy-MM-ddTHH:mm:ss.fzzz", "yyyy-MM-ddTHH:mm:ss.ff", "yyyy-MM-ddTHH:mm:ss.ffZ", "yyyy-MM-ddTHH:mm:ss.ffzzz", @"\-yyyy-MM-ddTHH:mm:ss.ff",
            @"\-yyyy-MM-ddTHH:mm:ss.ffZ", @"\-yyyy-MM-ddTHH:mm:ss.ffzzz", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.fffZ", "yyyy-MM-ddTHH:mm:ss.fffzzz", @"\-yyyy-MM-ddTHH:mm:ss.fff", @"\-yyyy-MM-ddTHH:mm:ss.fffZ", @"\-yyyy-MM-ddTHH:mm:ss.fffzzz", "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss.ffffZ", "yyyy-MM-ddTHH:mm:ss.ffffzzz", @"\-yyyy-MM-ddTHH:mm:ss.ffff", @"\-yyyy-MM-ddTHH:mm:ss.ffffZ", @"\-yyyy-MM-ddTHH:mm:ss.ffffzzz", "yyyy-MM-ddTHH:mm:ss.fffff", "yyyy-MM-ddTHH:mm:ss.fffffZ",
            "yyyy-MM-ddTHH:mm:ss.fffffzzz", @"\-yyyy-MM-ddTHH:mm:ss.fffff", @"\-yyyy-MM-ddTHH:mm:ss.fffffZ", @"\-yyyy-MM-ddTHH:mm:ss.fffffzzz", "yyyy-MM-ddTHH:mm:ss.ffffff", "yyyy-MM-ddTHH:mm:ss.ffffffZ", "yyyy-MM-ddTHH:mm:ss.ffffffzzz", @"\-yyyy-MM-ddTHH:mm:ss.ffffff", @"\-yyyy-MM-ddTHH:mm:ss.ffffffZ", @"\-yyyy-MM-ddTHH:mm:ss.ffffffzzz", "yyyy-MM-ddTHH:mm:ss.fffffff", "yyyy-MM-ddTHH:mm:ss.fffffffZ", "yyyy-MM-ddTHH:mm:ss.fffffffzzz", @"\-yyyy-MM-ddTHH:mm:ss.fffffff", @"\-yyyy-MM-ddTHH:mm:ss.fffffffZ", @"\-yyyy-MM-ddTHH:mm:ss.fffffffzzz"
        };
        private const string _defaultPropertyPartNameExtension = ".psmdcp";
        private const string _defaultPropertyPartNamePrefix = "/package/services/metadata/core-properties/";
        private bool _dirty;
        private const string _guidStorageFormatString = "N";
        private NameTable _nameTable;
        private const int _numCoreProperties = 0x10;
        private Package _package;
        private Dictionary<PackageXmlEnum, object> _propertyDictionary = new Dictionary<PackageXmlEnum, object>(0x10);
        private PackagePart _propertyPart;
        private static PackageXmlEnum[] _validProperties = new PackageXmlEnum[] { PackageXmlEnum.Creator, PackageXmlEnum.Identifier, PackageXmlEnum.Title, PackageXmlEnum.Subject, PackageXmlEnum.Description, PackageXmlEnum.Language, PackageXmlEnum.Created, PackageXmlEnum.Modified, PackageXmlEnum.ContentType, PackageXmlEnum.Keywords, PackageXmlEnum.Category, PackageXmlEnum.Version, PackageXmlEnum.LastModifiedBy, PackageXmlEnum.ContentStatus, PackageXmlEnum.Revision, PackageXmlEnum.LastPrinted };
        private const string _w3cdtf = "W3CDTF";
        private XmlTextWriter _xmlWriter;

        internal PartBasedPackageProperties(Package package)
        {
            this._package = package;
            this._nameTable = PackageXmlStringTable.NameTable;
            this.ReadPropertyValuesFromPackage();
            this._dirty = false;
        }

        internal void Close()
        {
            this.Flush();
            if (this._package.InStreamingCreation && (this._xmlWriter != null))
            {
                this.CloseXmlWriter();
            }
        }

        private void CloseXmlWriter()
        {
            this._xmlWriter.WriteEndElement();
            this._xmlWriter.Close();
            this._xmlWriter = null;
        }

        private void CreatePropertyPart()
        {
            this._propertyPart = this._package.CreatePart(this.GeneratePropertyPartUri(), _coreDocumentPropertiesContentType.ToString());
            this._package.CreateRelationship(this._propertyPart.Uri, TargetMode.Internal, "http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
        }

        private void EnsurePropertyPart()
        {
            if (this._propertyPart == null)
            {
                if ((this._package.FileOpenAccess == FileAccess.Read) || (this._package.FileOpenAccess == FileAccess.ReadWrite))
                {
                    this._propertyPart = this.GetPropertyPart();
                    if (this._propertyPart != null)
                    {
                        return;
                    }
                }
                this.CreatePropertyPart();
            }
        }

        private void EnsureXmlWriter()
        {
            if (!this._package.InStreamingCreation)
            {
                Invariant.Assert(this._xmlWriter == null);
            }
            if (this._xmlWriter == null)
            {
                Stream stream;
                this.EnsurePropertyPart();
                if (this._package.InStreamingCreation)
                {
                    stream = this._propertyPart.GetStream(FileMode.Create, FileAccess.Write);
                }
                else
                {
                    stream = new IgnoreFlushAndCloseStream(this._propertyPart.GetStream(FileMode.Create, FileAccess.Write));
                }
                this._xmlWriter = new XmlTextWriter(stream, Encoding.UTF8);
                this.WriteXmlStartTagsForPackageProperties();
            }
        }

        internal void Flush()
        {
            if (this._dirty)
            {
                this.EnsureXmlWriter();
                this.SerializeDirtyProperties();
                if (this._package.InStreamingCreation && (this._xmlWriter != null))
                {
                    this._xmlWriter.Flush();
                }
                else
                {
                    this.CloseXmlWriter();
                }
            }
        }

        private Uri GeneratePropertyPartUri() => 
            PackUriHelper.CreatePartUri(new Uri("/package/services/metadata/core-properties/" + Guid.NewGuid().ToString("N", null) + ".psmdcp", UriKind.Relative));

        private PackageRelationship GetCorePropertiesRelationship()
        {
            PackageRelationship relationship = null;
            foreach (PackageRelationship relationship2 in this._package.GetRelationshipsByType("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties"))
            {
                if (relationship != null)
                {
                    throw new FileFormatException(System.Windows.SR.Get("MoreThanOneMetadataRelationships"));
                }
                relationship = relationship2;
            }
            return relationship;
        }

        private DateTime? GetDateData(XmlTextReader reader)
        {
            DateTime time;
            string stringData = this.GetStringData(reader);
            try
            {
                time = XmlConvert.ToDateTime(stringData, _dateTimeFormats);
            }
            catch (FormatException exception)
            {
                throw new XmlException(System.Windows.SR.Get("XsdDateTimeExpected"), exception, reader.LineNumber, reader.LinePosition);
            }
            return new DateTime?(time);
        }

        private DateTime? GetDateTimePropertyValue(PackageXmlEnum propertyName)
        {
            object propertyValue = this.GetPropertyValue(propertyName);
            if (propertyValue == null)
            {
                return null;
            }
            return (DateTime?) propertyValue;
        }

        private PackagePart GetPropertyPart()
        {
            PackageRelationship corePropertiesRelationship = this.GetCorePropertiesRelationship();
            if (corePropertiesRelationship == null)
            {
                return null;
            }
            if (corePropertiesRelationship.TargetMode != TargetMode.Internal)
            {
                throw new FileFormatException(System.Windows.SR.Get("NoExternalTargetForMetadataRelationship"));
            }
            PackagePart part = null;
            Uri partUri = PackUriHelper.ResolvePartUri(PackUriHelper.PackageRootUri, corePropertiesRelationship.TargetUri);
            if (!this._package.PartExists(partUri))
            {
                throw new FileFormatException(System.Windows.SR.Get("DanglingMetadataRelationship"));
            }
            part = this._package.GetPart(partUri);
            if (!part.ValidatedContentType.AreTypeAndSubTypeEqual(_coreDocumentPropertiesContentType))
            {
                throw new FileFormatException(System.Windows.SR.Get("WrongContentTypeForPropertyPart"));
            }
            return part;
        }

        private object GetPropertyValue(PackageXmlEnum propertyName)
        {
            this._package.ThrowIfWriteOnly();
            if (!this._propertyDictionary.ContainsKey(propertyName))
            {
                return null;
            }
            return this._propertyDictionary[propertyName];
        }

        private string GetStringData(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return string.Empty;
            }
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.EndElement)
            {
                return string.Empty;
            }
            if (reader.NodeType != XmlNodeType.Text)
            {
                throw new XmlException(System.Windows.SR.Get("NoStructuredContentInsideProperties"), null, reader.LineNumber, reader.LinePosition);
            }
            return reader.Value;
        }

        private void ParseCorePropertyPart(PackagePart part)
        {
            XmlTextReader reader = new XmlTextReader(part.GetStream(FileMode.Open, FileAccess.Read), this._nameTable) {
                ProhibitDtd = true
            };
            PackagingUtilities.PerformInitailReadAndVerifyEncoding(reader);
            if (((reader.MoveToContent() != XmlNodeType.Element) || (reader.NamespaceURI != PackageXmlStringTable.GetXmlStringAsObject(PackageXmlEnum.PackageCorePropertiesNamespace))) || (reader.LocalName != PackageXmlStringTable.GetXmlStringAsObject(PackageXmlEnum.CoreProperties)))
            {
                throw new XmlException(System.Windows.SR.Get("CorePropertiesElementExpected"), null, reader.LineNumber, reader.LinePosition);
            }
            if (PackagingUtilities.GetNonXmlnsAttributeCount(reader) != 0)
            {
                throw new XmlException(System.Windows.SR.Get("PropertyWrongNumbOfAttribsDefinedOn", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
            }
            while (reader.Read() && (reader.MoveToContent() != XmlNodeType.None))
            {
                if (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        throw new XmlException(System.Windows.SR.Get("PropertyStartTagExpected"), null, reader.LineNumber, reader.LinePosition);
                    }
                    if (reader.Depth != 1)
                    {
                        throw new XmlException(System.Windows.SR.Get("NoStructuredContentInsideProperties"), null, reader.LineNumber, reader.LinePosition);
                    }
                    int nonXmlnsAttributeCount = PackagingUtilities.GetNonXmlnsAttributeCount(reader);
                    PackageXmlEnum enumOf = PackageXmlStringTable.GetEnumOf(reader.LocalName);
                    string valueType = PackageXmlStringTable.GetValueType(enumOf);
                    if (Array.IndexOf<PackageXmlEnum>(_validProperties, enumOf) == -1)
                    {
                        throw new XmlException(System.Windows.SR.Get("InvalidPropertyNameInCorePropertiesPart", new object[] { reader.LocalName }), null, reader.LineNumber, reader.LinePosition);
                    }
                    if (reader.NamespaceURI != PackageXmlStringTable.GetXmlStringAsObject(PackageXmlStringTable.GetXmlNamespace(enumOf)))
                    {
                        throw new XmlException(System.Windows.SR.Get("UnknownNamespaceInCorePropertiesPart"), null, reader.LineNumber, reader.LinePosition);
                    }
                    if (string.CompareOrdinal(valueType, "String") == 0)
                    {
                        if (nonXmlnsAttributeCount != 0)
                        {
                            throw new XmlException(System.Windows.SR.Get("PropertyWrongNumbOfAttribsDefinedOn", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
                        }
                        this.RecordNewBinding(enumOf, this.GetStringData(reader), true, reader);
                    }
                    else if (string.CompareOrdinal(valueType, "DateTime") == 0)
                    {
                        int num2 = (reader.NamespaceURI == PackageXmlStringTable.GetXmlStringAsObject(PackageXmlEnum.DublinCoreTermsNamespace)) ? 1 : 0;
                        if (nonXmlnsAttributeCount != num2)
                        {
                            throw new XmlException(System.Windows.SR.Get("PropertyWrongNumbOfAttribsDefinedOn", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
                        }
                        if (num2 != 0)
                        {
                            ValidateXsiType(reader, PackageXmlStringTable.GetXmlStringAsObject(PackageXmlEnum.DublinCoreTermsNamespace), "W3CDTF");
                        }
                        this.RecordNewBinding(enumOf, this.GetDateData(reader), true, reader);
                    }
                    else
                    {
                        Invariant.Assert(false, "Unknown value type for properties");
                    }
                }
            }
        }

        private void ReadPropertyValuesFromPackage()
        {
            Invariant.Assert(this._propertyPart == null);
            if (this._package.FileOpenAccess != FileAccess.Write)
            {
                this._propertyPart = this.GetPropertyPart();
                if (this._propertyPart != null)
                {
                    this.ParseCorePropertyPart(this._propertyPart);
                }
            }
        }

        private void RecordNewBinding(PackageXmlEnum propertyenum, object value)
        {
            this.RecordNewBinding(propertyenum, value, false, null);
        }

        private void RecordNewBinding(PackageXmlEnum propertyenum, object value, bool initializing, XmlTextReader reader)
        {
            if (!initializing)
            {
                this._package.ThrowIfReadOnly();
            }
            if (this._propertyDictionary.ContainsKey(propertyenum))
            {
                if (initializing)
                {
                    throw new XmlException(System.Windows.SR.Get("DuplicateCorePropertyName", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
                }
                if (this._package.InStreamingCreation)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("OperationViolatesWriteOnceSemantics"));
                }
                if (value == null)
                {
                    this._propertyDictionary.Remove(propertyenum);
                }
                else
                {
                    this._propertyDictionary[propertyenum] = value;
                }
                this._dirty = !initializing;
            }
            else if (value == null)
            {
                if (this._package.InStreamingCreation)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("OperationViolatesWriteOnceSemantics"));
                }
            }
            else
            {
                this._propertyDictionary.Add(propertyenum, value);
                this._dirty = !initializing;
            }
        }

        private void SerializeDirtyProperties()
        {
            KeyValuePair<PackageXmlEnum, object>[] pairArray = null;
            int num = 0;
            if (this._package.InStreamingCreation)
            {
                pairArray = new KeyValuePair<PackageXmlEnum, object>[this._propertyDictionary.Count];
            }
            foreach (KeyValuePair<PackageXmlEnum, object> pair in this._propertyDictionary)
            {
                if (!this._package.InStreamingCreation || (pair.Value != null))
                {
                    PackageXmlEnum xmlNamespace = PackageXmlStringTable.GetXmlNamespace(pair.Key);
                    this._xmlWriter.WriteStartElement(PackageXmlStringTable.GetXmlString(pair.Key), PackageXmlStringTable.GetXmlString(xmlNamespace));
                    if (pair.Value is DateTime?)
                    {
                        if (xmlNamespace == PackageXmlEnum.DublinCoreTermsNamespace)
                        {
                            this._xmlWriter.WriteStartAttribute(PackageXmlStringTable.GetXmlString(PackageXmlEnum.Type), PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlSchemaInstanceNamespace));
                            this._xmlWriter.WriteQualifiedName("W3CDTF", PackageXmlStringTable.GetXmlString(PackageXmlEnum.DublinCoreTermsNamespace));
                            this._xmlWriter.WriteEndAttribute();
                        }
                        DateTime? nullable = (DateTime?) pair.Value;
                        this._xmlWriter.WriteString(XmlConvert.ToString(nullable.Value.ToUniversalTime(), "yyyy-MM-ddTHH:mm:ss.fffffffZ"));
                    }
                    else
                    {
                        this._xmlWriter.WriteString(pair.Value.ToString());
                    }
                    this._xmlWriter.WriteEndElement();
                    if (this._package.InStreamingCreation)
                    {
                        pairArray[num++] = pair;
                    }
                }
            }
            this._dirty = false;
            if (this._package.InStreamingCreation)
            {
                for (int i = 0; i < num; i++)
                {
                    this._propertyDictionary[pairArray[i].Key] = null;
                }
            }
        }

        internal static void ValidateXsiType(XmlTextReader reader, object ns, string name)
        {
            string attribute = reader.GetAttribute(PackageXmlStringTable.GetXmlString(PackageXmlEnum.Type), PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlSchemaInstanceNamespace));
            if (attribute == null)
            {
                throw new XmlException(System.Windows.SR.Get("UnknownDCDateTimeXsiType", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
            }
            int index = attribute.IndexOf(':');
            if (index == -1)
            {
                throw new XmlException(System.Windows.SR.Get("UnknownDCDateTimeXsiType", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
            }
            if (!object.ReferenceEquals(ns, reader.LookupNamespace(attribute.Substring(0, index))) || (string.CompareOrdinal(name, attribute.Substring(index + 1, (attribute.Length - index) - 1)) != 0))
            {
                throw new XmlException(System.Windows.SR.Get("UnknownDCDateTimeXsiType", new object[] { reader.Name }), null, reader.LineNumber, reader.LinePosition);
            }
        }

        private void WriteXmlStartTagsForPackageProperties()
        {
            this._xmlWriter.WriteStartDocument();
            this._xmlWriter.WriteStartElement(PackageXmlStringTable.GetXmlString(PackageXmlEnum.CoreProperties), PackageXmlStringTable.GetXmlString(PackageXmlEnum.PackageCorePropertiesNamespace));
            this._xmlWriter.WriteAttributeString(PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlNamespacePrefix), PackageXmlStringTable.GetXmlString(PackageXmlEnum.DublinCorePropertiesNamespacePrefix), null, PackageXmlStringTable.GetXmlString(PackageXmlEnum.DublinCorePropertiesNamespace));
            this._xmlWriter.WriteAttributeString(PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlNamespacePrefix), PackageXmlStringTable.GetXmlString(PackageXmlEnum.DublincCoreTermsNamespacePrefix), null, PackageXmlStringTable.GetXmlString(PackageXmlEnum.DublinCoreTermsNamespace));
            this._xmlWriter.WriteAttributeString(PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlNamespacePrefix), PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlSchemaInstanceNamespacePrefix), null, PackageXmlStringTable.GetXmlString(PackageXmlEnum.XmlSchemaInstanceNamespace));
        }

        public override string Category
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Category));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Category, value);
            }
        }

        public override string ContentStatus
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.ContentStatus));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.ContentStatus, value);
            }
        }

        public override string ContentType
        {
            get => 
                (this.GetPropertyValue(PackageXmlEnum.ContentType) as string);
            set
            {
                this.RecordNewBinding(PackageXmlEnum.ContentType, value);
            }
        }

        public override DateTime? Created
        {
            get => 
                this.GetDateTimePropertyValue(PackageXmlEnum.Created);
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Created, value);
            }
        }

        public override string Creator
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Creator));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Creator, value);
            }
        }

        public override string Description
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Description));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Description, value);
            }
        }

        public override string Identifier
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Identifier));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Identifier, value);
            }
        }

        public override string Keywords
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Keywords));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Keywords, value);
            }
        }

        public override string Language
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Language));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Language, value);
            }
        }

        public override string LastModifiedBy
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.LastModifiedBy));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.LastModifiedBy, value);
            }
        }

        public override DateTime? LastPrinted
        {
            get => 
                this.GetDateTimePropertyValue(PackageXmlEnum.LastPrinted);
            set
            {
                this.RecordNewBinding(PackageXmlEnum.LastPrinted, value);
            }
        }

        public override DateTime? Modified
        {
            get => 
                this.GetDateTimePropertyValue(PackageXmlEnum.Modified);
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Modified, value);
            }
        }

        public override string Revision
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Revision));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Revision, value);
            }
        }

        public override string Subject
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Subject));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Subject, value);
            }
        }

        public override string Title
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Title));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Title, value);
            }
        }

        public override string Version
        {
            get => 
                ((string) this.GetPropertyValue(PackageXmlEnum.Version));
            set
            {
                this.RecordNewBinding(PackageXmlEnum.Version, value);
            }
        }
    }
}

