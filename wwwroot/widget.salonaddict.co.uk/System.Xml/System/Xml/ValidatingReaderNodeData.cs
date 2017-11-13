namespace System.Xml
{
    using System;

    internal class ValidatingReaderNodeData
    {
        private AttributePSVIInfo attributePSVIInfo;
        private int depth;
        private int lineNo;
        private int linePos;
        private string localName;
        private string namespaceUri;
        private string nameWPrefix;
        private XmlNodeType nodeType;
        private string originalStringValue;
        private string prefix;
        private string rawValue;

        public ValidatingReaderNodeData()
        {
            this.Clear(XmlNodeType.None);
        }

        public ValidatingReaderNodeData(XmlNodeType nodeType)
        {
            this.Clear(nodeType);
        }

        internal void Clear(XmlNodeType nodeType)
        {
            this.nodeType = nodeType;
            this.localName = string.Empty;
            this.prefix = string.Empty;
            this.namespaceUri = string.Empty;
            this.rawValue = string.Empty;
            if (this.attributePSVIInfo != null)
            {
                this.attributePSVIInfo.Reset();
            }
            this.nameWPrefix = null;
            this.lineNo = 0;
            this.linePos = 0;
        }

        internal void ClearName()
        {
            this.localName = string.Empty;
            this.prefix = string.Empty;
            this.namespaceUri = string.Empty;
        }

        public string GetAtomizedNameWPrefix(XmlNameTable nameTable)
        {
            if (this.nameWPrefix == null)
            {
                if (this.prefix.Length == 0)
                {
                    this.nameWPrefix = this.localName;
                }
                else
                {
                    this.nameWPrefix = nameTable.Add(this.prefix + ":" + this.localName);
                }
            }
            return this.nameWPrefix;
        }

        internal void SetItemData(string value)
        {
            this.SetItemData(value, value);
        }

        internal void SetItemData(string value, string originalStringValue)
        {
            this.rawValue = value;
            this.originalStringValue = originalStringValue;
        }

        internal void SetItemData(string localName, string prefix, string ns, int depth)
        {
            this.localName = localName;
            this.prefix = prefix;
            this.namespaceUri = ns;
            this.depth = depth;
            this.rawValue = string.Empty;
        }

        internal void SetItemData(string localName, string prefix, string ns, string value)
        {
            this.localName = localName;
            this.prefix = prefix;
            this.namespaceUri = ns;
            this.rawValue = value;
        }

        internal void SetLineInfo(IXmlLineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                this.lineNo = lineInfo.LineNumber;
                this.linePos = lineInfo.LinePosition;
            }
        }

        internal void SetLineInfo(int lineNo, int linePos)
        {
            this.lineNo = lineNo;
            this.linePos = linePos;
        }

        public AttributePSVIInfo AttInfo
        {
            get => 
                this.attributePSVIInfo;
            set
            {
                this.attributePSVIInfo = value;
            }
        }

        public int Depth
        {
            get => 
                this.depth;
            set
            {
                this.depth = value;
            }
        }

        public int LineNumber =>
            this.lineNo;

        public int LinePosition =>
            this.linePos;

        public string LocalName
        {
            get => 
                this.localName;
            set
            {
                this.localName = value;
            }
        }

        public string Namespace
        {
            get => 
                this.namespaceUri;
            set
            {
                this.namespaceUri = value;
            }
        }

        public XmlNodeType NodeType
        {
            get => 
                this.nodeType;
            set
            {
                this.nodeType = value;
            }
        }

        public string OriginalStringValue
        {
            get => 
                this.originalStringValue;
            set
            {
                this.originalStringValue = value;
            }
        }

        public string Prefix
        {
            get => 
                this.prefix;
            set
            {
                this.prefix = value;
            }
        }

        public string RawValue
        {
            get => 
                this.rawValue;
            set
            {
                this.rawValue = value;
            }
        }
    }
}

