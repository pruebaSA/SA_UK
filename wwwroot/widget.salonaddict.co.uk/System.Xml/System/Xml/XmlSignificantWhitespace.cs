﻿namespace System.Xml
{
    using System;
    using System.Xml.XPath;

    public class XmlSignificantWhitespace : XmlCharacterData
    {
        protected internal XmlSignificantWhitespace(string strData, XmlDocument doc) : base(strData, doc)
        {
            if (!doc.IsLoading && !base.CheckOnData(strData))
            {
                throw new ArgumentException(Res.GetString("Xdom_WS_Char"));
            }
        }

        public override XmlNode CloneNode(bool deep) => 
            this.OwnerDocument.CreateSignificantWhitespace(this.Data);

        public override void WriteContentTo(XmlWriter w)
        {
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteString(this.Data);
        }

        internal override bool IsText =>
            true;

        public override string LocalName =>
            this.OwnerDocument.strSignificantWhitespaceName;

        public override string Name =>
            this.OwnerDocument.strSignificantWhitespaceName;

        public override XmlNodeType NodeType =>
            XmlNodeType.SignificantWhitespace;

        public override XmlNode ParentNode
        {
            get
            {
                switch (base.parentNode.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                    {
                        XmlNode parentNode = base.parentNode.parentNode;
                        while (parentNode.IsText)
                        {
                            parentNode = parentNode.parentNode;
                        }
                        return parentNode;
                    }
                    case XmlNodeType.Document:
                        return base.ParentNode;
                }
                return base.parentNode;
            }
        }

        internal override XmlNode PreviousText
        {
            get
            {
                if (base.parentNode.IsText)
                {
                    return base.parentNode;
                }
                return null;
            }
        }

        public override string Value
        {
            get => 
                this.Data;
            set
            {
                if (!base.CheckOnData(value))
                {
                    throw new ArgumentException(Res.GetString("Xdom_WS_Char"));
                }
                this.Data = value;
            }
        }

        internal override XPathNodeType XPNodeType
        {
            get
            {
                XPathNodeType significantWhitespace = XPathNodeType.SignificantWhitespace;
                base.DecideXPNodeTypeForTextNodes(this, ref significantWhitespace);
                return significantWhitespace;
            }
        }
    }
}

