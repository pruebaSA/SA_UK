namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XslNode
    {
        public readonly object Arg;
        private List<XslNode> content;
        private static readonly IList<XslNode> EmptyList = new List<XslNode>().AsReadOnly();
        public XslFlags Flags;
        public readonly QilName Name;
        public NsDecl Namespaces;
        public readonly XslNodeType NodeType;
        public ISourceLineInfo SourceLine;
        public readonly System.Xml.Xsl.Xslt.XslVersion XslVersion;

        public XslNode(XslNodeType nodeType)
        {
            this.NodeType = nodeType;
            this.XslVersion = System.Xml.Xsl.Xslt.XslVersion.Version10;
        }

        public XslNode(XslNodeType nodeType, QilName name, object arg, System.Xml.Xsl.Xslt.XslVersion xslVer)
        {
            this.NodeType = nodeType;
            this.Name = name;
            this.Arg = arg;
            this.XslVersion = xslVer;
        }

        public void AddContent(XslNode node)
        {
            if (this.content == null)
            {
                this.content = new List<XslNode>();
            }
            this.content.Add(node);
        }

        public void InsertContent(IEnumerable<XslNode> collection)
        {
            if (this.content == null)
            {
                this.content = new List<XslNode>(collection);
            }
            else
            {
                this.content.InsertRange(0, collection);
            }
        }

        public void SetContent(List<XslNode> content)
        {
            this.content = content;
        }

        public IList<XslNode> Content =>
            (this.content ?? EmptyList);

        public bool ForwardsCompatible =>
            (this.XslVersion == System.Xml.Xsl.Xslt.XslVersion.ForwardsCompatible);

        public string Select =>
            ((string) this.Arg);

        internal string TraceName =>
            null;
    }
}

