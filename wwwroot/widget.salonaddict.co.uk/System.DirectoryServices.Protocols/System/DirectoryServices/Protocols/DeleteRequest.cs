namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Xml;

    public class DeleteRequest : DirectoryRequest
    {
        private string dn;

        public DeleteRequest()
        {
        }

        public DeleteRequest(string distinguishedName)
        {
            this.dn = distinguishedName;
        }

        protected override XmlElement ToXmlNode(XmlDocument doc) => 
            base.CreateRequestElement(doc, "delRequest", true, this.dn);

        public string DistinguishedName
        {
            get => 
                this.dn;
            set
            {
                this.dn = value;
            }
        }
    }
}

