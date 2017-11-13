namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Text;
    using System.Xml;

    internal class BuilderInfo
    {
        private int depth;
        internal HtmlAttributeProps htmlAttrProps;
        internal HtmlElementProps htmlProps;
        private bool isEmptyTag;
        private string localName;
        private string name;
        private string namespaceURI;
        private XmlNodeType nodeType;
        private string prefix;
        internal bool search;
        internal string[] TextInfo = new string[4];
        internal int TextInfoCount;

        internal BuilderInfo()
        {
            this.Initialize(string.Empty, string.Empty, string.Empty);
        }

        internal BuilderInfo Clone()
        {
            BuilderInfo info = new BuilderInfo();
            info.Initialize(this);
            return info;
        }

        private void EnsureTextInfoSize(int newSize)
        {
            if (this.TextInfo.Length < newSize)
            {
                string[] destinationArray = new string[newSize * 2];
                Array.Copy(this.TextInfo, destinationArray, this.TextInfoCount);
                this.TextInfo = destinationArray;
            }
        }

        internal void Initialize(BuilderInfo src)
        {
            this.prefix = src.Prefix;
            this.localName = src.LocalName;
            this.namespaceURI = src.NamespaceURI;
            this.name = null;
            this.depth = src.Depth;
            this.nodeType = src.NodeType;
            this.htmlProps = src.htmlProps;
            this.htmlAttrProps = src.htmlAttrProps;
            this.TextInfoCount = 0;
            this.EnsureTextInfoSize(src.TextInfoCount);
            src.TextInfo.CopyTo(this.TextInfo, 0);
            this.TextInfoCount = src.TextInfoCount;
        }

        internal void Initialize(string prefix, string name, string nspace)
        {
            this.prefix = prefix;
            this.localName = name;
            this.namespaceURI = nspace;
            this.name = null;
            this.htmlProps = null;
            this.htmlAttrProps = null;
            this.TextInfoCount = 0;
        }

        internal void ValueAppend(string s, bool disableEscaping)
        {
            if ((s != null) && (s.Length != 0))
            {
                this.EnsureTextInfoSize(this.TextInfoCount + (disableEscaping ? 2 : 1));
                if (disableEscaping)
                {
                    this.TextInfo[this.TextInfoCount++] = null;
                }
                this.TextInfo[this.TextInfoCount++] = s;
            }
        }

        internal int Depth
        {
            get => 
                this.depth;
            set
            {
                this.depth = value;
            }
        }

        internal bool IsEmptyTag
        {
            get => 
                this.isEmptyTag;
            set
            {
                this.isEmptyTag = value;
            }
        }

        internal string LocalName
        {
            get => 
                this.localName;
            set
            {
                this.localName = value;
            }
        }

        internal string Name
        {
            get
            {
                if (this.name == null)
                {
                    string prefix = this.Prefix;
                    string localName = this.LocalName;
                    if ((prefix != null) && (0 < prefix.Length))
                    {
                        if (localName.Length > 0)
                        {
                            this.name = prefix + ":" + localName;
                        }
                        else
                        {
                            this.name = prefix;
                        }
                    }
                    else
                    {
                        this.name = localName;
                    }
                }
                return this.name;
            }
        }

        internal string NamespaceURI
        {
            get => 
                this.namespaceURI;
            set
            {
                this.namespaceURI = value;
            }
        }

        internal XmlNodeType NodeType
        {
            get => 
                this.nodeType;
            set
            {
                this.nodeType = value;
            }
        }

        internal string Prefix
        {
            get => 
                this.prefix;
            set
            {
                this.prefix = value;
            }
        }

        internal string Value
        {
            get
            {
                switch (this.TextInfoCount)
                {
                    case 0:
                        return string.Empty;

                    case 1:
                        return this.TextInfo[0];
                }
                int capacity = 0;
                for (int i = 0; i < this.TextInfoCount; i++)
                {
                    string str = this.TextInfo[i];
                    if (str != null)
                    {
                        capacity += str.Length;
                    }
                }
                StringBuilder builder = new StringBuilder(capacity);
                for (int j = 0; j < this.TextInfoCount; j++)
                {
                    string str2 = this.TextInfo[j];
                    if (str2 != null)
                    {
                        builder.Append(str2);
                    }
                }
                return builder.ToString();
            }
            set
            {
                this.TextInfoCount = 0;
                this.ValueAppend(value, false);
            }
        }
    }
}

