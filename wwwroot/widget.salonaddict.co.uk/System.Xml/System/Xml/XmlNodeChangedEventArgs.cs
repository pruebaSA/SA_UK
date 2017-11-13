namespace System.Xml
{
    using System;

    public class XmlNodeChangedEventArgs : EventArgs
    {
        private XmlNodeChangedAction action;
        private XmlNode newParent;
        private string newValue;
        private XmlNode node;
        private XmlNode oldParent;
        private string oldValue;

        public XmlNodeChangedEventArgs(XmlNode node, XmlNode oldParent, XmlNode newParent, string oldValue, string newValue, XmlNodeChangedAction action)
        {
            this.node = node;
            this.oldParent = oldParent;
            this.newParent = newParent;
            this.action = action;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public XmlNodeChangedAction Action =>
            this.action;

        public XmlNode NewParent =>
            this.newParent;

        public string NewValue =>
            this.newValue;

        public XmlNode Node =>
            this.node;

        public XmlNode OldParent =>
            this.oldParent;

        public string OldValue =>
            this.oldValue;
    }
}

