namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilName : QilLiteral
    {
        private string local;
        private string prefix;
        private string uri;

        public QilName(QilNodeType nodeType, string local, string uri, string prefix) : base(nodeType, null)
        {
            this.LocalName = local;
            this.NamespaceUri = uri;
            this.Prefix = prefix;
            base.Value = this;
        }

        public override bool Equals(object other)
        {
            QilName name = other as QilName;
            if (name == null)
            {
                return false;
            }
            return ((this.local == name.local) && (this.uri == name.uri));
        }

        public override int GetHashCode() => 
            this.local.GetHashCode();

        public override string ToString()
        {
            if (this.prefix.Length == 0)
            {
                if (this.uri.Length == 0)
                {
                    return this.local;
                }
                return ("{" + this.uri + "}" + this.local);
            }
            return ("{" + this.uri + "}" + this.prefix + ":" + this.local);
        }

        public string LocalName
        {
            get => 
                this.local;
            set
            {
                this.local = value;
            }
        }

        public string NamespaceUri
        {
            get => 
                this.uri;
            set
            {
                this.uri = value;
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

        public string QualifiedName
        {
            get
            {
                if (this.prefix.Length == 0)
                {
                    return this.local;
                }
                return (this.prefix + ':' + this.local);
            }
        }
    }
}

