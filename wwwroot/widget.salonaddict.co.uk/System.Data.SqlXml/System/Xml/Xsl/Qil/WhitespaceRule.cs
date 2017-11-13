namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Xml.Xsl.Runtime;

    internal class WhitespaceRule
    {
        private string localName;
        private string namespaceName;
        private bool preserveSpace;

        protected WhitespaceRule()
        {
        }

        public WhitespaceRule(XmlQueryDataReader reader)
        {
            this.localName = reader.ReadStringQ();
            this.namespaceName = reader.ReadStringQ();
            this.preserveSpace = reader.ReadBoolean();
        }

        public WhitespaceRule(string localName, string namespaceName, bool preserveSpace)
        {
            this.Init(localName, namespaceName, preserveSpace);
        }

        public void GetObjectData(XmlQueryDataWriter writer)
        {
            writer.WriteStringQ(this.localName);
            writer.WriteStringQ(this.namespaceName);
            writer.Write(this.preserveSpace);
        }

        protected void Init(string localName, string namespaceName, bool preserveSpace)
        {
            this.localName = localName;
            this.namespaceName = namespaceName;
            this.preserveSpace = preserveSpace;
        }

        public string LocalName
        {
            get => 
                this.localName;
            set
            {
                this.localName = value;
            }
        }

        public string NamespaceName
        {
            get => 
                this.namespaceName;
            set
            {
                this.namespaceName = value;
            }
        }

        public bool PreserveSpace =>
            this.preserveSpace;
    }
}

