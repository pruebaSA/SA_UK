namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class XsltInput : IErrorHelper
    {
        private KeywordsTable atoms;
        private Compiler compiler;
        private SourceLineInfo lastLineInfo;
        private Moves lastMove = Moves.Child;
        private bool lastResult = true;
        private string localName;
        private string[] names = new string[10];
        private string namespaceName;
        private XPathNodeType nodeType;
        private XmlReader reader;
        private IXmlLineInfo readerLineInfo;
        private InputScopeManager scopeManager;
        private string text;
        private bool textIsWhite;
        private bool textPreserveWS;
        private int textStartLine;
        private int textStartPos;
        private bool topLevelReader;
        private string value;
        private string[] values = new string[10];
        private static XPathNodeType[] XmlNodeType2XPathNodeType = new XPathNodeType[] { 
            ~XPathNodeType.Root, XPathNodeType.Element, XPathNodeType.Attribute, XPathNodeType.Text, XPathNodeType.Text, ~XPathNodeType.Root, ~XPathNodeType.Root, XPathNodeType.ProcessingInstruction, XPathNodeType.Comment, ~XPathNodeType.Root, ~XPathNodeType.Root, ~XPathNodeType.Root, ~XPathNodeType.Root, XPathNodeType.Whitespace, XPathNodeType.SignificantWhitespace, XPathNodeType.Element,
            ~XPathNodeType.Root, ~XPathNodeType.Root
        };

        public XsltInput(XmlReader reader, Compiler compiler)
        {
            EnsureExpandEntities(reader);
            IXmlLineInfo info = reader as IXmlLineInfo;
            this.reader = reader;
            this.readerLineInfo = ((info != null) && info.HasLineInfo()) ? info : null;
            this.topLevelReader = reader.ReadState == ReadState.Initial;
            this.scopeManager = new InputScopeManager(reader.NameTable);
            this.atoms = new KeywordsTable(reader.NameTable);
            this.compiler = compiler;
            this.textIsWhite = true;
            this.nodeType = XPathNodeType.Root;
        }

        public void AddExtensionNamespace(string uri)
        {
            this.scopeManager.AddExtensionNamespace(uri);
        }

        public ISourceLineInfo BuildLineInfo()
        {
            bool flag = this.nodeType == XPathNodeType.Attribute;
            if ((this.lastLineInfo != null) && !flag)
            {
                return this.lastLineInfo;
            }
            SourceLineInfo info = new SourceLineInfo(this.Uri, this.StartLine, this.StartPos, this.EndLine, this.EndPos);
            if (!this.OnTextNode && !flag)
            {
                this.lastLineInfo = info;
            }
            return info;
        }

        private static XPathNodeType ConvertNodeType(XmlNodeType xmlNodeType)
        {
            XPathNodeType type = XmlNodeType2XPathNodeType[(int) xmlNodeType];
            if (type == ~XPathNodeType.Root)
            {
                return XPathNodeType.All;
            }
            return type;
        }

        private static void EnsureExpandEntities(XmlReader reader)
        {
            XmlTextReader reader2 = reader as XmlTextReader;
            if ((reader2 != null) && (reader2.EntityHandling != EntityHandling.ExpandEntities))
            {
                reader2.EntityHandling = EntityHandling.ExpandEntities;
            }
        }

        public void Finish()
        {
            this.reader.Read();
            this.FixLastLineInfo();
            if (this.topLevelReader)
            {
                while (this.reader.ReadState == ReadState.Interactive)
                {
                    this.reader.Skip();
                }
            }
        }

        public void FixLastLineInfo()
        {
            if (this.lastLineInfo != null)
            {
                this.lastLineInfo.SetEndLinePos(this.StartLine, this.StartPos);
                this.lastLineInfo = null;
            }
        }

        public ContextInfo GetAttributes() => 
            this.GetAttributes(0, 0, this.names, this.values);

        public ContextInfo GetAttributes(int required, string name, out string value)
        {
            this.names[0] = name;
            ContextInfo info = this.GetAttributes(required, 1, this.names, this.values);
            value = this.values[0];
            return info;
        }

        public ContextInfo GetAttributes(int required, int number, string[] names, string[] values)
        {
            for (int i = 0; i < number; i++)
            {
                values[i] = null;
            }
            string qualifiedName = this.QualifiedName;
            ContextInfo info = new ContextInfo(this);
            this.compiler.EnterForwardsCompatible();
            while (this.MoveToNextAttOrNs())
            {
                if (this.nodeType == XPathNodeType.Namespace)
                {
                    info.AddNamespace(this);
                    continue;
                }
                info.AddAttribute(this);
                bool flag = false;
                for (int k = 0; k < number; k++)
                {
                    if (this.IsXsltAttribute(names[k]))
                    {
                        flag = true;
                        values[k] = this.Value;
                        if (Ref.Equal(names[k], this.Atoms.Version) && (k < required))
                        {
                            this.SetVersion(this.Value, this.Atoms.Version);
                        }
                        break;
                    }
                }
                if (!flag && (this.IsNullNamespace() || this.IsXsltNamespace()))
                {
                    this.ReportError("Xslt_InvalidAttribute", new string[] { this.QualifiedName, qualifiedName });
                }
            }
            this.compiler.ExitForwardsCompatible(this.ForwardCompatibility);
            for (int j = 0; j < required; j++)
            {
                if (values[j] == null)
                {
                    this.ReportError("Xslt_MissingAttribute", new string[] { names[j] });
                }
            }
            info.Finish(this);
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            ContextInfo info = this.GetAttributes(required, 2, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1, string name2, out string value2)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            this.names[2] = name2;
            ContextInfo info = this.GetAttributes(required, 3, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            value2 = this.values[2];
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1, string name2, out string value2, string name3, out string value3)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            this.names[2] = name2;
            this.names[3] = name3;
            ContextInfo info = this.GetAttributes(required, 4, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            value2 = this.values[2];
            value3 = this.values[3];
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1, string name2, out string value2, string name3, out string value3, string name4, out string value4)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            this.names[2] = name2;
            this.names[3] = name3;
            this.names[4] = name4;
            ContextInfo info = this.GetAttributes(required, 5, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            value2 = this.values[2];
            value3 = this.values[3];
            value4 = this.values[4];
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1, string name2, out string value2, string name3, out string value3, string name4, out string value4, string name5, out string value5, string name6, out string value6, string name7, out string value7, string name8, out string value8)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            this.names[2] = name2;
            this.names[3] = name3;
            this.names[4] = name4;
            this.names[5] = name5;
            this.names[6] = name6;
            this.names[7] = name7;
            this.names[8] = name8;
            ContextInfo info = this.GetAttributes(required, 9, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            value2 = this.values[2];
            value3 = this.values[3];
            value4 = this.values[4];
            value5 = this.values[5];
            value6 = this.values[6];
            value7 = this.values[7];
            value8 = this.values[8];
            return info;
        }

        public ContextInfo GetAttributes(int required, string name0, out string value0, string name1, out string value1, string name2, out string value2, string name3, out string value3, string name4, out string value4, string name5, out string value5, string name6, out string value6, string name7, out string value7, string name8, out string value8, string name9, out string value9)
        {
            this.names[0] = name0;
            this.names[1] = name1;
            this.names[2] = name2;
            this.names[3] = name3;
            this.names[4] = name4;
            this.names[5] = name5;
            this.names[6] = name6;
            this.names[7] = name7;
            this.names[8] = name8;
            this.names[9] = name9;
            ContextInfo info = this.GetAttributes(required, 10, this.names, this.values);
            value0 = this.values[0];
            value1 = this.values[1];
            value2 = this.values[2];
            value3 = this.values[3];
            value4 = this.values[4];
            value5 = this.values[5];
            value6 = this.values[6];
            value7 = this.values[7];
            value8 = this.values[8];
            value9 = this.values[9];
            return info;
        }

        public bool IsExtensionNamespace(string uri) => 
            this.scopeManager.IsExtensionNamespace(uri);

        public bool IsKeyword(string kwd) => 
            Ref.Equal(kwd, this.LocalName);

        private static bool IsNamespace(XmlReader reader) => 
            ((reader.Prefix == "xmlns") || ((reader.Prefix.Length == 0) && (reader.LocalName == "xmlns")));

        public bool IsNs(string ns) => 
            Ref.Equal(ns, this.NamespaceUri);

        public bool IsNullNamespace() => 
            this.IsNs(string.Empty);

        private static bool IsWhitespace(string text) => 
            XmlCharType.Instance.IsOnlyWhitespace(text);

        public bool IsXsltAttribute(string kwd) => 
            (this.IsKeyword(kwd) && this.IsNullNamespace());

        public bool IsXsltNamespace() => 
            this.IsNs(this.atoms.UriXsl);

        public string LookupXmlNamespace(string prefix)
        {
            string array = this.reader.LookupNamespace(prefix);
            if (array != null)
            {
                return this.NameTable.Add(array);
            }
            if (prefix.Length == 0)
            {
                return string.Empty;
            }
            this.ReportError("Xslt_InvalidPrefix", new string[] { prefix });
            return null;
        }

        public bool MoveToFirstChild()
        {
            bool flag = this.MoveToFirstChildAny();
            if (flag && ((this.NodeType == XPathNodeType.Comment) || (this.NodeType == XPathNodeType.ProcessingInstruction)))
            {
                flag = this.MoveToNextSibling();
                if (!flag)
                {
                    this.MoveToParent();
                }
            }
            return flag;
        }

        private bool MoveToFirstChildAny()
        {
            if (!this.reader.IsEmptyElement)
            {
                return this.ReadNextSibling();
            }
            this.nodeType = XPathNodeType.Element;
            return false;
        }

        public bool MoveToNextAttOrNs()
        {
            if (this.NodeType == XPathNodeType.Element)
            {
                if (!this.reader.MoveToFirstAttribute())
                {
                    this.reader.MoveToElement();
                    return false;
                }
            }
            else if (!this.reader.MoveToNextAttribute())
            {
                this.reader.MoveToElement();
                this.nodeType = XPathNodeType.Element;
                return false;
            }
            this.SetCachedProperties();
            return true;
        }

        public bool MoveToNextSibling()
        {
            bool flag;
            do
            {
                this.StepOffNode();
                flag = this.ReadNextSibling();
            }
            while (flag && ((this.NodeType == XPathNodeType.Comment) || (this.NodeType == XPathNodeType.ProcessingInstruction)));
            return flag;
        }

        public bool MoveToParent() => 
            true;

        private static int PositionAdjustment(XmlNodeType nt)
        {
            switch (nt)
            {
                case XmlNodeType.CDATA:
                    return 9;

                case XmlNodeType.ProcessingInstruction:
                    return 2;

                case XmlNodeType.Comment:
                    return 4;

                case XmlNodeType.EndElement:
                    return 2;

                case XmlNodeType.Element:
                    return 1;
            }
            return 0;
        }

        private bool ReadNextSibling()
        {
            bool flag = this.ReadNextSiblingHelper();
            this.FixLastLineInfo();
            if (flag)
            {
                this.StepOnNodeRdr();
                return true;
            }
            this.nodeType = XPathNodeType.Element;
            return false;
        }

        private bool ReadNextSiblingHelper()
        {
            if (this.text == null)
            {
                goto Label_00FA;
            }
            this.text = null;
            this.textIsWhite = true;
            return (this.reader.NodeType != XmlNodeType.EndElement);
        Label_0094:
            if (((this.reader.Depth != 0) || (this.text != null)) || !this.textIsWhite)
            {
                if (this.text == null)
                {
                    this.SaveTextInfo();
                }
                this.text = this.text + this.reader.Value;
            }
        Label_00FA:
            if (this.reader.Read())
            {
                switch (this.reader.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        if (this.textIsWhite && !IsWhitespace(this.reader.Value))
                        {
                            this.textIsWhite = false;
                        }
                        goto Label_0094;

                    case XmlNodeType.EntityReference:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.XmlDeclaration:
                        goto Label_00FA;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        goto Label_0094;
                }
                return ((this.text != null) || (this.reader.NodeType != XmlNodeType.EndElement));
            }
            return (this.text != null);
        }

        public void ReportError(string res, params string[] args)
        {
            this.compiler.ReportError(this.BuildLineInfo(), res, args);
        }

        public void ReportWarning(string res, params string[] args)
        {
            this.compiler.ReportWarning(this.BuildLineInfo(), res, args);
        }

        private void SaveTextInfo()
        {
            this.textPreserveWS = this.reader.XmlSpace == XmlSpace.Preserve;
            this.textStartLine = this.StartLine;
            this.textStartPos = this.StartPos;
        }

        private void SetCachedProperties()
        {
            this.nodeType = ConvertNodeType(this.reader.NodeType);
            this.localName = this.reader.LocalName;
            this.namespaceName = this.reader.NamespaceURI;
            this.value = this.reader.Value;
            if ((this.nodeType == XPathNodeType.Attribute) && IsNamespace(this.reader))
            {
                this.nodeType = XPathNodeType.Namespace;
                this.namespaceName = string.Empty;
                if (this.localName == "xmlns")
                {
                    this.localName = string.Empty;
                }
            }
        }

        [Conditional("DEBUG")]
        private void SetLastMove(Moves lastMove, bool lastResult)
        {
            this.lastMove = lastMove;
            this.lastResult = lastResult;
        }

        public void SetVersion(string version, string attName)
        {
            double d = XPathConvert.StringToDouble(version);
            if (double.IsNaN(d))
            {
                this.ReportError("Xslt_InvalidAttrValue", new string[] { attName, version });
                d = 1.0;
            }
            this.scopeManager.ForwardCompatibility = d != 1.0;
        }

        public void SkipNode()
        {
            if ((this.NodeType == XPathNodeType.Element) && this.MoveToFirstChild())
            {
                do
                {
                    this.SkipNode();
                }
                while (this.MoveToNextSibling());
                this.MoveToParent();
            }
        }

        public bool Start()
        {
            if (!this.topLevelReader)
            {
                if (this.reader.ReadState != ReadState.Interactive)
                {
                    return false;
                }
                this.StepOnNodeRdr();
                if ((this.nodeType != XPathNodeType.Comment) && (this.nodeType != XPathNodeType.ProcessingInstruction))
                {
                    return (this.nodeType == XPathNodeType.Element);
                }
            }
            return this.MoveToNextSibling();
        }

        private void StepOffNode()
        {
            if (this.NodeType == XPathNodeType.Element)
            {
                this.scopeManager.PopScope();
            }
        }

        private void StepOnNodeRdr()
        {
            if (this.text == null)
            {
                this.SetCachedProperties();
            }
            else
            {
                this.value = this.text;
                this.localName = string.Empty;
                this.namespaceName = string.Empty;
                this.nodeType = !this.textIsWhite ? XPathNodeType.Text : (this.textPreserveWS ? XPathNodeType.SignificantWhitespace : XPathNodeType.Whitespace);
            }
            if (this.NodeType == XPathNodeType.Element)
            {
                this.scopeManager.PushScope();
            }
        }

        public KeywordsTable Atoms =>
            this.atoms;

        public string BaseUri =>
            this.reader.BaseURI;

        public bool CanHaveApplyImports
        {
            get => 
                this.scopeManager.CanHaveApplyImports;
            set
            {
                this.scopeManager.CanHaveApplyImports = value;
            }
        }

        public int EndLine =>
            this.readerLineInfo?.LineNumber;

        public int EndPos
        {
            get
            {
                if (this.readerLineInfo == null)
                {
                    return 0;
                }
                int linePosition = this.readerLineInfo.LinePosition;
                if (this.OnTextNode)
                {
                    return (linePosition - PositionAdjustment(this.reader.NodeType));
                }
                return (linePosition + 1);
            }
        }

        public bool ForwardCompatibility =>
            this.scopeManager.ForwardCompatibility;

        public bool IsEmptyElement =>
            this.reader.IsEmptyElement;

        public string LocalName =>
            this.localName;

        public string NamespaceUri =>
            this.namespaceName;

        public XmlNameTable NameTable =>
            this.reader.NameTable;

        public XPathNodeType NodeType =>
            this.nodeType;

        private bool OnTextNode =>
            (this.text != null);

        public string Prefix =>
            this.reader.Prefix;

        public string QualifiedName =>
            this.reader.Name;

        public int StartLine
        {
            get
            {
                if (this.readerLineInfo == null)
                {
                    return 0;
                }
                if (this.OnTextNode)
                {
                    return this.textStartLine;
                }
                return this.readerLineInfo.LineNumber;
            }
        }

        public int StartPos
        {
            get
            {
                if (this.readerLineInfo == null)
                {
                    return 0;
                }
                if (this.OnTextNode)
                {
                    return this.textStartPos;
                }
                return (this.readerLineInfo.LinePosition - PositionAdjustment(this.reader.NodeType));
            }
        }

        public string Uri =>
            this.reader.BaseURI;

        public string Value =>
            this.value;

        public System.Xml.Xsl.Xslt.XslVersion XslVersion
        {
            get
            {
                if (!this.scopeManager.ForwardCompatibility)
                {
                    return System.Xml.Xsl.Xslt.XslVersion.Version10;
                }
                return System.Xml.Xsl.Xslt.XslVersion.ForwardsCompatible;
            }
        }

        internal class ContextInfo
        {
            private int elemNameLength;
            public ISourceLineInfo elemNameLi;
            public ISourceLineInfo endTagLi;
            public ISourceLineInfo lineInfo;
            public NsDecl nsList;

            public ContextInfo(XsltInput input)
            {
                this.lineInfo = input.BuildLineInfo();
                this.elemNameLength = input.QualifiedName.Length;
            }

            public void AddAttribute(XsltInput input)
            {
            }

            public void AddNamespace(XsltInput input)
            {
                if (!Ref.Equal(input.LocalName, input.Atoms.Xml))
                {
                    this.nsList = new NsDecl(this.nsList, input.LocalName, input.NameTable.Add(input.Value));
                }
            }

            public void Finish(XsltInput input)
            {
            }

            public void SaveExtendedLineInfo(XsltInput input)
            {
                this.elemNameLi = new SourceLineInfo(this.lineInfo.Uri, this.lineInfo.StartLine, this.lineInfo.StartPos + 1, this.lineInfo.StartLine, (this.lineInfo.StartPos + 1) + this.elemNameLength);
                if (!input.IsEmptyElement)
                {
                    this.endTagLi = input.BuildLineInfo();
                }
                else
                {
                    this.endTagLi = new EmptyElementEndTag(this.lineInfo);
                }
            }

            internal class EmptyElementEndTag : ISourceLineInfo
            {
                private ISourceLineInfo elementTagLi;

                public EmptyElementEndTag(ISourceLineInfo elementTagLi)
                {
                    this.elementTagLi = elementTagLi;
                }

                public int EndLine =>
                    this.elementTagLi.EndLine;

                public int EndPos =>
                    this.elementTagLi.EndPos;

                public bool IsNoSource =>
                    this.elementTagLi.IsNoSource;

                public int StartLine =>
                    this.elementTagLi.EndLine;

                public int StartPos =>
                    (this.elementTagLi.EndPos - 2);

                public string Uri =>
                    this.elementTagLi.Uri;
            }
        }

        private class InputScopeManager
        {
            private int lastRecord;
            private int lastScopes;
            private XmlNameTable nameTable;
            private ScopeRecord[] records = new ScopeRecord[0x20];

            public InputScopeManager(XmlNameTable nameTable)
            {
                this.nameTable = nameTable;
                this.records[0].scopeFlags = (ScopeFlags) 0;
            }

            public void AddExtensionNamespace(string uri)
            {
                uri = this.nameTable.Add(uri);
                ScopeFlags scopeFlags = this.records[this.lastRecord].scopeFlags;
                if ((this.lastScopes != 0) || ((scopeFlags & ScopeFlags.NsExtension) != ((ScopeFlags) 0)))
                {
                    this.AddRecord();
                    scopeFlags &= ScopeFlags.InheritedFlags;
                }
                this.records[this.lastRecord].nsUri = uri;
                this.records[this.lastRecord].scopeFlags = scopeFlags | ScopeFlags.NsExtension;
            }

            private void AddRecord()
            {
                this.records[this.lastRecord].scopeCount = this.lastScopes;
                this.lastRecord++;
                if (this.lastRecord == this.records.Length)
                {
                    ScopeRecord[] destinationArray = new ScopeRecord[this.lastRecord * 2];
                    Array.Copy(this.records, 0, destinationArray, 0, this.lastRecord);
                    this.records = destinationArray;
                }
                this.lastScopes = 0;
            }

            [Conditional("DEBUG")]
            public void CheckEmpty()
            {
                this.PopScope();
            }

            public bool IsExtensionNamespace(string nsUri)
            {
                for (int i = this.lastRecord; 0 <= i; i--)
                {
                    if (((this.records[i].scopeFlags & ScopeFlags.NsExtension) != ((ScopeFlags) 0)) && (this.records[i].nsUri == nsUri))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void PopScope()
            {
                if (0 < this.lastScopes)
                {
                    this.lastScopes--;
                }
                else
                {
                    do
                    {
                        this.lastRecord--;
                    }
                    while (this.records[this.lastRecord].scopeCount == 0);
                    this.lastScopes = this.records[this.lastRecord].scopeCount;
                    this.lastScopes--;
                }
            }

            public void PushScope()
            {
                this.lastScopes++;
            }

            private void SetFlag(bool value, ScopeFlags flag)
            {
                ScopeFlags scopeFlags = this.records[this.lastRecord].scopeFlags;
                if (((scopeFlags & flag) != ((ScopeFlags) 0)) != value)
                {
                    if (this.lastScopes != 0)
                    {
                        this.AddRecord();
                        scopeFlags &= ScopeFlags.InheritedFlags;
                    }
                    this.records[this.lastRecord].scopeFlags = scopeFlags ^ flag;
                }
            }

            public bool CanHaveApplyImports
            {
                get => 
                    ((this.records[this.lastRecord].scopeFlags & ScopeFlags.CanHaveApplyImports) != ((ScopeFlags) 0));
                set
                {
                    this.SetFlag(value, ScopeFlags.CanHaveApplyImports);
                }
            }

            public bool ForwardCompatibility
            {
                get => 
                    ((this.records[this.lastRecord].scopeFlags & ScopeFlags.ForwardCompatibility) != ((ScopeFlags) 0));
                set
                {
                    this.SetFlag(value, ScopeFlags.ForwardCompatibility);
                }
            }

            private enum ScopeFlags
            {
                CanHaveApplyImports = 2,
                ForwardCompatibility = 1,
                InheritedFlags = 3,
                NsExtension = 4
            }

            [StructLayout(LayoutKind.Sequential)]
            private struct ScopeRecord
            {
                public int scopeCount;
                public XsltInput.InputScopeManager.ScopeFlags scopeFlags;
                public string nsUri;
            }
        }

        private enum Moves
        {
            Next,
            Child,
            Parent
        }
    }
}

