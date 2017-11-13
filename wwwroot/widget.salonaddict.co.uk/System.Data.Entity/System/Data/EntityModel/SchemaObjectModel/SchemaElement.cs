namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;

    [DebuggerDisplay("Name={Name}")]
    internal abstract class SchemaElement
    {
        private DocumentationElement _documentation;
        private int _lineNumber;
        private int _linePosition;
        private string _name;
        private List<MetadataProperty> _otherContent;
        private SchemaElement _parentElement;
        private System.Data.EntityModel.SchemaObjectModel.Schema _schema;
        protected const int MaxValueVersionComponent = 0x7fff;
        internal const string XmlNamespaceNamespace = "http://www.w3.org/2000/xmlns/";

        internal SchemaElement(SchemaElement parentElement)
        {
            if (parentElement != null)
            {
                this.ParentElement = parentElement;
                for (SchemaElement element = parentElement; element != null; element = element.ParentElement)
                {
                    System.Data.EntityModel.SchemaObjectModel.Schema schema = element as System.Data.EntityModel.SchemaObjectModel.Schema;
                    if (schema != null)
                    {
                        this.Schema = schema;
                        break;
                    }
                }
                if (this.Schema == null)
                {
                    throw EntityUtil.InvalidOperation(Strings.AllElementsMustBeInSchema);
                }
            }
        }

        internal SchemaElement(SchemaElement parentElement, string name) : this(parentElement)
        {
            this._name = name;
        }

        internal void AddAlreadyDefinedError(XmlReader reader)
        {
            this.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, reader, Strings.AlreadyDefined(reader.Name));
        }

        internal void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, object message)
        {
            this.AddError(errorCode, severity, this.SchemaLocation, this.LineNumber, this.LinePosition, message);
        }

        internal void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, SchemaElement element, object message)
        {
            this.AddError(errorCode, severity, element.Schema.Location, element.LineNumber, element.LinePosition, message);
        }

        internal void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, XmlReader reader, object message)
        {
            int num;
            int num2;
            GetPositionInfo(reader, out num, out num2);
            this.AddError(errorCode, severity, this.SchemaLocation, num, num2, message);
        }

        internal void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, int lineNumber, int linePosition, object message)
        {
            this.AddError(errorCode, severity, this.SchemaLocation, lineNumber, linePosition, message);
        }

        private void AddError(ErrorCode errorCode, EdmSchemaErrorSeverity severity, string sourceLocation, int lineNumber, int linePosition, object message)
        {
            EdmSchemaError error = null;
            string str = message as string;
            if (str != null)
            {
                error = new EdmSchemaError(str, (int) errorCode, severity, sourceLocation, lineNumber, linePosition);
            }
            else
            {
                Exception exception = message as Exception;
                if (exception != null)
                {
                    error = new EdmSchemaError(exception.Message, (int) errorCode, severity, sourceLocation, lineNumber, linePosition, exception);
                }
                else
                {
                    error = new EdmSchemaError(message.ToString(), (int) errorCode, severity, sourceLocation, lineNumber, linePosition);
                }
            }
            this.Schema.AddError(error);
        }

        private bool AddOtherContent(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                return true;
            }
            if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/")
            {
                return true;
            }
            MetadataProperty item = CreateMetadataPropertyFromOtherNamespaceXmlAttribute(reader.NamespaceURI, reader.LocalName, reader.Value);
            this.OtherContent.Add(item);
            return false;
        }

        [Conditional("DEBUG")]
        internal static void AssertReaderConsidersSchemaInvalid(XmlReader reader)
        {
        }

        internal static bool CanHandleAttribute(XmlReader reader, string localName) => 
            ((reader.NamespaceURI.Length == 0) && (reader.LocalName == localName));

        protected bool CanHandleElement(XmlReader reader, string localName) => 
            ((reader.NamespaceURI == this.Schema.SchemaXmlNamespace) && (reader.LocalName == localName));

        internal virtual SchemaElement Clone(SchemaElement parentElement)
        {
            throw Error.NotImplemented();
        }

        internal static MetadataProperty CreateMetadataPropertyFromOtherNamespaceXmlAttribute(string xmlNamespaceUri, string attributeName, string value) => 
            new MetadataProperty(xmlNamespaceUri + ":" + attributeName, TypeUsage.Create(EdmProviderManifest.Instance.GetPrimitiveType(PrimitiveTypeKind.String)), value);

        internal void GetPositionInfo(XmlReader reader)
        {
            GetPositionInfo(reader, out this._lineNumber, out this._linePosition);
        }

        internal static void GetPositionInfo(XmlReader reader, out int lineNumber, out int linePosition)
        {
            IXmlLineInfo info = reader as IXmlLineInfo;
            if ((info != null) && info.HasLineInfo())
            {
                lineNumber = info.LineNumber;
                linePosition = info.LinePosition;
            }
            else
            {
                lineNumber = 0;
                linePosition = 0;
            }
        }

        protected virtual bool HandleAttribute(XmlReader reader)
        {
            if (CanHandleAttribute(reader, "Name"))
            {
                this.HandleNameAttribute(reader);
                return true;
            }
            return false;
        }

        protected virtual void HandleAttributesComplete()
        {
        }

        internal bool HandleBoolAttribute(XmlReader reader, ref bool field)
        {
            bool flag;
            if (!Utils.GetBool(this.Schema, reader, out flag))
            {
                return false;
            }
            field = flag;
            return true;
        }

        internal bool HandleByteAttribute(XmlReader reader, ref byte field)
        {
            byte num;
            if (!Utils.GetByte(this.Schema, reader, out num))
            {
                return false;
            }
            field = num;
            return true;
        }

        protected virtual void HandleChildElementsComplete()
        {
        }

        private void HandleDocumentationElement(XmlReader reader)
        {
            if (this.Documentation != null)
            {
                this.AddAlreadyDefinedError(reader);
            }
            this.Documentation = new DocumentationElement(this);
            this.Documentation.Parse(reader);
        }

        protected ReturnValue<string> HandleDottedNameAttribute(XmlReader reader, string field, Func<object, string> errorFormat)
        {
            string str;
            ReturnValue<string> value2 = new ReturnValue<string>();
            if (!string.IsNullOrEmpty(field))
            {
                this.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, reader, errorFormat(reader.Name));
                return value2;
            }
            if (Utils.GetDottedName(this.Schema, reader, out str))
            {
                value2.Value = str;
            }
            return value2;
        }

        protected virtual bool HandleElement(XmlReader reader)
        {
            if (this.CanHandleElement(reader, "Documentation"))
            {
                this.HandleDocumentationElement(reader);
                return true;
            }
            return false;
        }

        internal bool HandleIntAttribute(XmlReader reader, ref int field)
        {
            int num;
            if (!Utils.GetInt(this.Schema, reader, out num))
            {
                return false;
            }
            field = num;
            return true;
        }

        protected virtual void HandleNameAttribute(XmlReader reader)
        {
            this.Name = this.HandleUndottedNameAttribute(reader, this.Name);
        }

        protected virtual bool HandleText(XmlReader reader) => 
            false;

        protected string HandleUndottedNameAttribute(XmlReader reader, string field)
        {
            string name = field;
            if (!string.IsNullOrEmpty(field))
            {
                this.AddAlreadyDefinedError(reader);
                return name;
            }
            if (!Utils.GetUndottedName(this.Schema, reader, out name))
            {
                return name;
            }
            return name;
        }

        internal void Parse(XmlReader reader)
        {
            this.GetPositionInfo(reader);
            bool flag = !reader.IsEmptyElement;
            for (bool flag2 = reader.MoveToFirstAttribute(); flag2; flag2 = reader.MoveToNextAttribute())
            {
                this.ParseAttribute(reader);
            }
            this.HandleAttributesComplete();
            bool flag3 = !flag;
            bool flag4 = false;
            while (!flag3)
            {
                if (flag4)
                {
                    flag4 = false;
                    reader.Skip();
                    if (reader.EOF)
                    {
                        break;
                    }
                }
                else if (!reader.Read())
                {
                    break;
                }
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        flag4 = this.ParseElement(reader);
                        continue;
                    }
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.SignificantWhitespace:
                    {
                        this.ParseText(reader);
                        continue;
                    }
                    case XmlNodeType.EntityReference:
                    case XmlNodeType.DocumentType:
                    {
                        flag4 = true;
                        continue;
                    }
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Comment:
                    case XmlNodeType.Notation:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.XmlDeclaration:
                    {
                        continue;
                    }
                    case XmlNodeType.EndElement:
                    {
                        flag3 = true;
                        continue;
                    }
                }
                this.AddError(ErrorCode.UnexpectedXmlNodeType, EdmSchemaErrorSeverity.Error, reader, Strings.UnexpectedXmlNodeType(reader.NodeType));
                flag4 = true;
            }
            this.HandleChildElementsComplete();
            if (reader.EOF && (reader.Depth > 0))
            {
                this.AddError(ErrorCode.MalformedXml, EdmSchemaErrorSeverity.Error, 0, 0, Strings.MalformedXml(this.LineNumber, this.LinePosition));
            }
        }

        private void ParseAttribute(XmlReader reader)
        {
            string namespaceURI = reader.NamespaceURI;
            if (!this.Schema.IsParseableXmlNamespace(namespaceURI, true))
            {
                this.AddOtherContent(reader);
            }
            else if ((this.ProhibitAttribute(namespaceURI, reader.LocalName) || !this.HandleAttribute(reader)) && (((reader.SchemaInfo == null) || (reader.SchemaInfo.Validity != XmlSchemaValidity.Invalid)) && (string.IsNullOrEmpty(namespaceURI) || this.Schema.IsParseableXmlNamespace(namespaceURI, true))))
            {
                this.AddError(ErrorCode.UnexpectedXmlAttribute, EdmSchemaErrorSeverity.Error, reader, Strings.UnexpectedXmlAttribute(reader.Name));
            }
        }

        private bool ParseElement(XmlReader reader)
        {
            string namespaceURI = reader.NamespaceURI;
            if (!this.Schema.IsParseableXmlNamespace(namespaceURI, true))
            {
                return this.AddOtherContent(reader);
            }
            if (this.HandleElement(reader))
            {
                return false;
            }
            if (string.IsNullOrEmpty(namespaceURI) || this.Schema.IsParseableXmlNamespace(reader.NamespaceURI, false))
            {
                this.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, reader, Strings.UnexpectedXmlElement(reader.Name));
            }
            return true;
        }

        private void ParseText(XmlReader reader)
        {
            if (!this.HandleText(reader) && ((reader.Value == null) || (reader.Value.Trim().Length != 0)))
            {
                this.AddError(ErrorCode.TextNotAllowed, EdmSchemaErrorSeverity.Error, reader, Strings.TextNotAllowed(reader.Value));
            }
        }

        protected virtual bool ProhibitAttribute(string namespaceUri, string localName) => 
            false;

        internal virtual void ResolveSecondLevelNames()
        {
        }

        internal virtual void ResolveTopLevelNames()
        {
        }

        protected virtual void SkipThroughElement(XmlReader reader)
        {
            this.Parse(reader);
        }

        internal static bool TrySplitExtendedMetadataPropertyName(string name, out string xmlNamespaceUri, out string attributeName)
        {
            int length = name.LastIndexOf(':');
            if ((length < 0) || (name.Length <= (length + 1)))
            {
                xmlNamespaceUri = null;
                attributeName = null;
                return false;
            }
            xmlNamespaceUri = name.Substring(0, length);
            attributeName = name.Substring(length + 1, (name.Length - 1) - length);
            return true;
        }

        internal virtual void Validate()
        {
        }

        internal DocumentationElement Documentation
        {
            get => 
                this._documentation;
            set
            {
                this._documentation = value;
            }
        }

        public virtual string FQName =>
            this.Name;

        public virtual string Identity =>
            this.Name;

        internal int LineNumber =>
            this._lineNumber;

        internal int LinePosition =>
            this._linePosition;

        public virtual string Name
        {
            get => 
                this._name;
            set
            {
                this._name = value;
            }
        }

        public List<MetadataProperty> OtherContent
        {
            get
            {
                if (this._otherContent == null)
                {
                    this._otherContent = new List<MetadataProperty>();
                }
                return this._otherContent;
            }
        }

        internal SchemaElement ParentElement
        {
            get => 
                this._parentElement;
            private set
            {
                this._parentElement = value;
            }
        }

        internal System.Data.EntityModel.SchemaObjectModel.Schema Schema
        {
            get => 
                this._schema;
            set
            {
                this._schema = value;
            }
        }

        protected string SchemaLocation
        {
            get
            {
                if (this.Schema != null)
                {
                    return this.Schema.Location;
                }
                return null;
            }
        }
    }
}

