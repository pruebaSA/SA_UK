namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    internal class XmlDataNode : DataNode<object>
    {
        private XmlDocument ownerDocument;
        private IList<System.Xml.XmlAttribute> xmlAttributes;
        private IList<System.Xml.XmlNode> xmlChildNodes;

        internal XmlDataNode()
        {
            base.dataType = Globals.TypeOfXmlDataNode;
        }

        public override void Clear()
        {
            base.Clear();
            this.xmlAttributes = null;
            this.xmlChildNodes = null;
            this.ownerDocument = null;
        }

        internal XmlDocument OwnerDocument
        {
            get => 
                this.ownerDocument;
            set
            {
                this.ownerDocument = value;
            }
        }

        internal IList<System.Xml.XmlAttribute> XmlAttributes
        {
            get => 
                this.xmlAttributes;
            set
            {
                this.xmlAttributes = value;
            }
        }

        internal IList<System.Xml.XmlNode> XmlChildNodes
        {
            get => 
                this.xmlChildNodes;
            set
            {
                this.xmlChildNodes = value;
            }
        }
    }
}

