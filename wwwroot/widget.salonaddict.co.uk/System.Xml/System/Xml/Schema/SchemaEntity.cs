namespace System.Xml.Schema
{
    using System;
    using System.Xml;

    internal sealed class SchemaEntity
    {
        private string baseURI;
        private string declaredURI;
        private bool isDeclaredInExternal;
        private bool isExternal;
        private bool isParameter;
        private bool isProcessed;
        private int lineNumber;
        private int linePosition;
        private XmlQualifiedName name;
        private XmlQualifiedName ndata = XmlQualifiedName.Empty;
        private string pubid;
        private string text;
        private string url;

        internal SchemaEntity(XmlQualifiedName name, bool isParameter)
        {
            this.name = name;
            this.isParameter = isParameter;
        }

        internal static bool IsPredefinedEntity(string n)
        {
            if (((n != "lt") && (n != "gt")) && ((n != "amp") && (n != "apos")))
            {
                return (n == "quot");
            }
            return true;
        }

        internal string BaseURI
        {
            get
            {
                if (this.baseURI != null)
                {
                    return this.baseURI;
                }
                return string.Empty;
            }
            set
            {
                this.baseURI = value;
            }
        }

        internal bool DeclaredInExternal
        {
            get => 
                this.isDeclaredInExternal;
            set
            {
                this.isDeclaredInExternal = value;
            }
        }

        internal string DeclaredURI
        {
            get
            {
                if (this.declaredURI != null)
                {
                    return this.declaredURI;
                }
                return string.Empty;
            }
            set
            {
                this.declaredURI = value;
            }
        }

        internal bool IsExternal
        {
            get => 
                this.isExternal;
            set
            {
                this.isExternal = value;
            }
        }

        internal bool IsParEntity
        {
            get => 
                this.isParameter;
            set
            {
                this.isParameter = value;
            }
        }

        internal bool IsProcessed
        {
            get => 
                this.isProcessed;
            set
            {
                this.isProcessed = value;
            }
        }

        internal int Line
        {
            get => 
                this.lineNumber;
            set
            {
                this.lineNumber = value;
            }
        }

        internal XmlQualifiedName Name =>
            this.name;

        internal XmlQualifiedName NData
        {
            get => 
                this.ndata;
            set
            {
                this.ndata = value;
            }
        }

        internal int Pos
        {
            get => 
                this.linePosition;
            set
            {
                this.linePosition = value;
            }
        }

        internal string Pubid
        {
            get => 
                this.pubid;
            set
            {
                this.pubid = value;
            }
        }

        internal string Text
        {
            get => 
                this.text;
            set
            {
                this.text = value;
                this.isExternal = false;
            }
        }

        internal string Url
        {
            get => 
                this.url;
            set
            {
                this.url = value;
                this.isExternal = true;
            }
        }
    }
}

