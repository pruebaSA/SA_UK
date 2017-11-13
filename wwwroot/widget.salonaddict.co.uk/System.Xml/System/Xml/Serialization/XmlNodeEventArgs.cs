namespace System.Xml.Serialization
{
    using System;
    using System.Xml;

    public class XmlNodeEventArgs : EventArgs
    {
        private int lineNumber;
        private int linePosition;
        private object o;
        private XmlNode xmlNode;

        internal XmlNodeEventArgs(XmlNode xmlNode, int lineNumber, int linePosition, object o)
        {
            this.o = o;
            this.xmlNode = xmlNode;
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }

        public int LineNumber =>
            this.lineNumber;

        public int LinePosition =>
            this.linePosition;

        public string LocalName =>
            this.xmlNode.LocalName;

        public string Name =>
            this.xmlNode.Name;

        public string NamespaceURI =>
            this.xmlNode.NamespaceURI;

        public XmlNodeType NodeType =>
            this.xmlNode.NodeType;

        public object ObjectBeingDeserialized =>
            this.o;

        public string Text =>
            this.xmlNode.Value;
    }
}

