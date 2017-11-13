namespace System.Xml
{
    using System;

    public class XmlNotation : XmlNode
    {
        private string name;
        private string publicId;
        private string systemId;

        internal XmlNotation(string name, string publicId, string systemId, XmlDocument doc) : base(doc)
        {
            this.name = doc.NameTable.Add(name);
            this.publicId = publicId;
            this.systemId = systemId;
        }

        public override XmlNode CloneNode(bool deep)
        {
            throw new InvalidOperationException(Res.GetString("Xdom_Node_Cloning"));
        }

        public override void WriteContentTo(XmlWriter w)
        {
        }

        public override void WriteTo(XmlWriter w)
        {
        }

        public override string InnerXml
        {
            get => 
                string.Empty;
            set
            {
                throw new InvalidOperationException(Res.GetString("Xdom_Set_InnerXml"));
            }
        }

        public override bool IsReadOnly =>
            true;

        public override string LocalName =>
            this.name;

        public override string Name =>
            this.name;

        public override XmlNodeType NodeType =>
            XmlNodeType.Notation;

        public override string OuterXml =>
            string.Empty;

        public string PublicId =>
            this.publicId;

        public string SystemId =>
            this.systemId;
    }
}

