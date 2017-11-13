namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;

    internal class WhitespaceRuleReader : XmlWrappingReader
    {
        private bool preserveAdjacent;
        private bool shouldStrip;
        private BitStack stkStrip;
        private string val;
        private WhitespaceRuleLookup wsRules;
        private XmlCharType xmlCharType;

        private WhitespaceRuleReader(XmlReader baseReader, WhitespaceRuleLookup wsRules) : base(baseReader)
        {
            this.xmlCharType = XmlCharType.Instance;
            this.val = null;
            this.stkStrip = new BitStack();
            this.shouldStrip = false;
            this.preserveAdjacent = false;
            this.wsRules = wsRules;
            this.wsRules.Atomize(baseReader.NameTable);
        }

        public static XmlReader CreateReader(XmlReader baseReader, WhitespaceRuleLookup wsRules)
        {
            if (wsRules == null)
            {
                return baseReader;
            }
            XmlReaderSettings settings = baseReader.Settings;
            if (settings != null)
            {
                if (settings.IgnoreWhitespace)
                {
                    return baseReader;
                }
            }
            else
            {
                XmlTextReader reader = baseReader as XmlTextReader;
                if ((reader != null) && (reader.WhitespaceHandling == WhitespaceHandling.None))
                {
                    return baseReader;
                }
                XmlTextReaderImpl impl = baseReader as XmlTextReaderImpl;
                if ((impl != null) && (impl.WhitespaceHandling == WhitespaceHandling.None))
                {
                    return baseReader;
                }
            }
            return new WhitespaceRuleReader(baseReader, wsRules);
        }

        public override bool Read()
        {
            XmlCharType instance = XmlCharType.Instance;
            string str = null;
            this.val = null;
            while (base.Read())
            {
                switch (base.NodeType)
                {
                    case XmlNodeType.Element:
                        if (!base.IsEmptyElement)
                        {
                            this.stkStrip.PushBit(this.shouldStrip);
                            this.shouldStrip = this.wsRules.ShouldStripSpace(base.LocalName, base.NamespaceURI) && (base.XmlSpace != XmlSpace.Preserve);
                        }
                        goto Label_012E;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        if (!this.preserveAdjacent)
                        {
                            break;
                        }
                        return true;

                    case XmlNodeType.EntityReference:
                        base.reader.ResolveEntity();
                        goto Label_012E;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        goto Label_00F6;

                    case XmlNodeType.EndElement:
                        this.shouldStrip = this.stkStrip.PopBit();
                        goto Label_012E;

                    case XmlNodeType.EndEntity:
                    {
                        continue;
                    }
                    default:
                        goto Label_012E;
                }
                if (!this.shouldStrip)
                {
                    goto Label_012E;
                }
                if (!instance.IsOnlyWhitespace(base.Value))
                {
                    if (str != null)
                    {
                        this.val = str + base.Value;
                    }
                    this.preserveAdjacent = true;
                    return true;
                }
            Label_00F6:
                if (this.preserveAdjacent)
                {
                    return true;
                }
                if (this.shouldStrip)
                {
                    if (str == null)
                    {
                        str = base.Value;
                    }
                    else
                    {
                        str = str + base.Value;
                    }
                    continue;
                }
            Label_012E:
                this.preserveAdjacent = false;
                return true;
            }
            return false;
        }

        public override string Value
        {
            get
            {
                if (this.val != null)
                {
                    return this.val;
                }
                return base.Value;
            }
        }
    }
}

