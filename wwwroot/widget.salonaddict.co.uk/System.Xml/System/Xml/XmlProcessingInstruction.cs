﻿namespace System.Xml
{
    using System;
    using System.Xml.XPath;

    public class XmlProcessingInstruction : XmlLinkedNode
    {
        private string data;
        private string target;

        protected internal XmlProcessingInstruction(string target, string data, XmlDocument doc) : base(doc)
        {
            this.target = target;
            this.data = data;
        }

        public override XmlNode CloneNode(bool deep) => 
            this.OwnerDocument.CreateProcessingInstruction(this.target, this.data);

        public override void WriteContentTo(XmlWriter w)
        {
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteProcessingInstruction(this.target, this.data);
        }

        public string Data
        {
            get => 
                this.data;
            set
            {
                XmlNode parentNode = this.ParentNode;
                XmlNodeChangedEventArgs args = this.GetEventArgs(this, parentNode, parentNode, this.data, value, XmlNodeChangedAction.Change);
                if (args != null)
                {
                    this.BeforeEvent(args);
                }
                this.data = value;
                if (args != null)
                {
                    this.AfterEvent(args);
                }
            }
        }

        public override string InnerText
        {
            get => 
                this.data;
            set
            {
                this.Data = value;
            }
        }

        public override string LocalName =>
            this.Name;

        public override string Name
        {
            get
            {
                if (this.target != null)
                {
                    return this.target;
                }
                return string.Empty;
            }
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.ProcessingInstruction;

        public string Target =>
            this.target;

        public override string Value
        {
            get => 
                this.data;
            set
            {
                this.Data = value;
            }
        }

        internal override string XPLocalName =>
            this.Name;

        internal override XPathNodeType XPNodeType =>
            XPathNodeType.ProcessingInstruction;
    }
}

