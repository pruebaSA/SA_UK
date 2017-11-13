namespace System.Xml
{
    using System;
    using System.Text;

    public class XmlDeclaration : XmlLinkedNode
    {
        private string encoding;
        private const string NO = "no";
        private string standalone;
        private const string VERNUM = "1.0";
        private const string YES = "yes";

        protected internal XmlDeclaration(string version, string encoding, string standalone, XmlDocument doc) : base(doc)
        {
            if (version != "1.0")
            {
                throw new ArgumentException(Res.GetString("Xdom_Version"));
            }
            if (((standalone != null) && (standalone.Length > 0)) && ((standalone != "yes") && (standalone != "no")))
            {
                throw new ArgumentException(Res.GetString("Xdom_standalone", new object[] { standalone }));
            }
            this.Encoding = encoding;
            this.Standalone = standalone;
        }

        public override XmlNode CloneNode(bool deep) => 
            this.OwnerDocument.CreateXmlDeclaration(this.Version, this.Encoding, this.Standalone);

        public override void WriteContentTo(XmlWriter w)
        {
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteProcessingInstruction(this.Name, this.InnerText);
        }

        public string Encoding
        {
            get => 
                this.encoding;
            set
            {
                this.encoding = (value == null) ? string.Empty : value;
            }
        }

        public override string InnerText
        {
            get
            {
                StringBuilder builder = new StringBuilder("version=\"" + this.Version + "\"");
                if (this.Encoding.Length > 0)
                {
                    builder.Append(" encoding=\"");
                    builder.Append(this.Encoding);
                    builder.Append("\"");
                }
                if (this.Standalone.Length > 0)
                {
                    builder.Append(" standalone=\"");
                    builder.Append(this.Standalone);
                    builder.Append("\"");
                }
                return builder.ToString();
            }
            set
            {
                string version = null;
                string encoding = null;
                string standalone = null;
                string str4 = this.Encoding;
                string str5 = this.Standalone;
                XmlLoader.ParseXmlDeclarationValue(value, out version, out encoding, out standalone);
                try
                {
                    if ((version != null) && (version != "1.0"))
                    {
                        throw new ArgumentException(Res.GetString("Xdom_Version"));
                    }
                    if (encoding != null)
                    {
                        this.Encoding = encoding;
                    }
                    if (standalone != null)
                    {
                        this.Standalone = standalone;
                    }
                }
                catch
                {
                    this.Encoding = str4;
                    this.Standalone = str5;
                    throw;
                }
            }
        }

        public override string LocalName =>
            this.Name;

        public override string Name =>
            "xml";

        public override XmlNodeType NodeType =>
            XmlNodeType.XmlDeclaration;

        public string Standalone
        {
            get => 
                this.standalone;
            set
            {
                if (value == null)
                {
                    this.standalone = string.Empty;
                }
                else
                {
                    if (((value.Length != 0) && (value != "yes")) && (value != "no"))
                    {
                        throw new ArgumentException(Res.GetString("Xdom_standalone", new object[] { value }));
                    }
                    this.standalone = value;
                }
            }
        }

        public override string Value
        {
            get => 
                this.InnerText;
            set
            {
                this.InnerText = value;
            }
        }

        public string Version =>
            "1.0";
    }
}

