namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class OutputScope : DocumentScope
    {
        private System.Xml.Xsl.XsltOld.HtmlElementProps htmlElementProps;
        private string lang;
        private bool mixed;
        private string name;
        private string nsUri;
        private string prefix;
        private XmlSpace space;
        private bool toCData;

        internal OutputScope()
        {
            this.Init(string.Empty, string.Empty, string.Empty, XmlSpace.None, string.Empty, false);
        }

        internal bool FindPrefix(string urn, out string prefix)
        {
            for (System.Xml.Xsl.XsltOld.NamespaceDecl decl = base.scopes; decl != null; decl = decl.Next)
            {
                if ((Keywords.Equals(decl.Uri, urn) && (decl.Prefix != null)) && (decl.Prefix.Length > 0))
                {
                    prefix = decl.Prefix;
                    return true;
                }
            }
            prefix = string.Empty;
            return false;
        }

        internal void Init(string name, string nspace, string prefix, XmlSpace space, string lang, bool mixed)
        {
            base.scopes = null;
            this.name = name;
            this.nsUri = nspace;
            this.prefix = prefix;
            this.space = space;
            this.lang = lang;
            this.mixed = mixed;
            this.toCData = false;
            this.htmlElementProps = null;
        }

        internal System.Xml.Xsl.XsltOld.HtmlElementProps HtmlElementProps
        {
            get => 
                this.htmlElementProps;
            set
            {
                this.htmlElementProps = value;
            }
        }

        internal string Lang
        {
            get => 
                this.lang;
            set
            {
                this.lang = value;
            }
        }

        internal bool Mixed
        {
            get => 
                this.mixed;
            set
            {
                this.mixed = value;
            }
        }

        internal string Name =>
            this.name;

        internal string Namespace =>
            this.nsUri;

        internal string Prefix
        {
            get => 
                this.prefix;
            set
            {
                this.prefix = value;
            }
        }

        internal XmlSpace Space
        {
            get => 
                this.space;
            set
            {
                this.space = value;
            }
        }

        internal bool ToCData
        {
            get => 
                this.toCData;
            set
            {
                this.toCData = value;
            }
        }
    }
}

