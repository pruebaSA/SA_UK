namespace System.IdentityModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal sealed class XmlTokenStream : ISecurityElement
    {
        private int count;
        private XmlTokenEntry[] entries;
        private string excludedElement;
        private string excludedElementNamespace;
        private int position;

        public XmlTokenStream(int initialSize)
        {
            if (initialSize < 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("initialSize", System.IdentityModel.SR.GetString("ValueMustBeGreaterThanZero")));
            }
            this.entries = new XmlTokenEntry[initialSize];
        }

        public void Add(XmlNodeType type, string value)
        {
            this.EnsureCapacityToAdd();
            this.entries[this.count++].Set(type, value);
        }

        public void AddAttribute(string prefix, string localName, string namespaceUri, string value)
        {
            this.EnsureCapacityToAdd();
            this.entries[this.count++].SetAttribute(prefix, localName, namespaceUri, value);
        }

        public void AddElement(string prefix, string localName, string namespaceUri, bool isEmptyElement)
        {
            this.EnsureCapacityToAdd();
            this.entries[this.count++].SetElement(prefix, localName, namespaceUri, isEmptyElement);
        }

        private void EnsureCapacityToAdd()
        {
            if (this.count == this.entries.Length)
            {
                XmlTokenEntry[] destinationArray = new XmlTokenEntry[this.entries.Length * 2];
                Array.Copy(this.entries, 0, destinationArray, 0, this.count);
                this.entries = destinationArray;
            }
        }

        public bool MoveToFirst()
        {
            this.position = 0;
            return (this.count > 0);
        }

        public bool MoveToFirstAttribute()
        {
            if ((this.position < (this.count - 1)) && (this.entries[this.position + 1].nodeType == XmlNodeType.Attribute))
            {
                this.position++;
                return true;
            }
            return false;
        }

        public bool MoveToNext()
        {
            if (this.position < (this.count - 1))
            {
                this.position++;
                return true;
            }
            return false;
        }

        public bool MoveToNextAttribute()
        {
            if ((this.position < (this.count - 1)) && (this.entries[this.position + 1].nodeType == XmlNodeType.Attribute))
            {
                this.position++;
                return true;
            }
            return false;
        }

        public void SetElementExclusion(string excludedElement, string excludedElementNamespace)
        {
            this.excludedElement = excludedElement;
            this.excludedElementNamespace = excludedElementNamespace;
        }

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("writer"));
            }
            if (!this.MoveToFirst())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.IdentityModel.SR.GetString("XmlTokenBufferIsEmpty")));
            }
            int num = 0;
            int num2 = -1;
            bool flag = true;
        Label_0040:
            switch (this.NodeType)
            {
                case XmlNodeType.Element:
                {
                    bool isEmptyElement = this.IsEmptyElement;
                    num++;
                    if ((flag && (this.LocalName == this.excludedElement)) && (this.NamespaceUri == this.excludedElementNamespace))
                    {
                        flag = false;
                        num2 = num;
                    }
                    if (flag)
                    {
                        writer.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceUri);
                    }
                    if (this.MoveToFirstAttribute())
                    {
                        do
                        {
                            if (flag)
                            {
                                writer.WriteAttributeString(this.Prefix, this.LocalName, this.NamespaceUri, this.Value);
                            }
                        }
                        while (this.MoveToNextAttribute());
                    }
                    if (!isEmptyElement)
                    {
                        goto Label_017C;
                    }
                    break;
                }
                case XmlNodeType.Text:
                    if (flag)
                    {
                        writer.WriteString(this.Value);
                    }
                    goto Label_017C;

                case XmlNodeType.CDATA:
                    if (flag)
                    {
                        writer.WriteCData(this.Value);
                    }
                    goto Label_017C;

                case XmlNodeType.Comment:
                    if (flag)
                    {
                        writer.WriteComment(this.Value);
                    }
                    goto Label_017C;

                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if (flag)
                    {
                        writer.WriteWhitespace(this.Value);
                    }
                    goto Label_017C;

                case XmlNodeType.EndElement:
                    break;

                default:
                    goto Label_017C;
            }
            if (flag)
            {
                writer.WriteEndElement();
            }
            else if (num2 == num)
            {
                flag = true;
                num2 = -1;
            }
            num--;
        Label_017C:
            if (this.MoveToNext())
            {
                goto Label_0040;
            }
        }

        public int Count =>
            this.count;

        public string ExcludedElement =>
            this.excludedElement;

        public string ExcludedElementNamespace =>
            this.excludedElementNamespace;

        public bool IsEmptyElement =>
            this.entries[this.position].IsEmptyElement;

        public string LocalName =>
            this.entries[this.position].localName;

        public string NamespaceUri =>
            this.entries[this.position].namespaceUri;

        public XmlNodeType NodeType =>
            this.entries[this.position].nodeType;

        public int Position =>
            this.position;

        public string Prefix =>
            this.entries[this.position].prefix;

        bool ISecurityElement.HasId =>
            false;

        string ISecurityElement.Id =>
            null;

        public string Value =>
            this.entries[this.position].Value;

        [StructLayout(LayoutKind.Sequential)]
        private struct XmlTokenEntry
        {
            internal XmlNodeType nodeType;
            internal string prefix;
            internal string localName;
            internal string namespaceUri;
            private string value;
            public bool IsEmptyElement
            {
                get => 
                    (this.value == null);
                set
                {
                    this.value = value ? null : "";
                }
            }
            public string Value =>
                this.value;
            public void Set(XmlNodeType nodeType, string value)
            {
                this.nodeType = nodeType;
                this.value = value;
            }

            public void SetAttribute(string prefix, string localName, string namespaceUri, string value)
            {
                this.nodeType = XmlNodeType.Attribute;
                this.prefix = prefix;
                this.localName = localName;
                this.namespaceUri = namespaceUri;
                this.value = value;
            }

            public void SetElement(string prefix, string localName, string namespaceUri, bool isEmptyElement)
            {
                this.nodeType = XmlNodeType.Element;
                this.prefix = prefix;
                this.localName = localName;
                this.namespaceUri = namespaceUri;
                this.IsEmptyElement = isEmptyElement;
            }
        }
    }
}

