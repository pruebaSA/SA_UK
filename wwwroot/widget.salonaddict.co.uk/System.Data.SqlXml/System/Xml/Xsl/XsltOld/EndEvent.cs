namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class EndEvent : Event
    {
        private XPathNodeType nodeType;

        internal EndEvent(XPathNodeType nodeType)
        {
            this.nodeType = nodeType;
        }

        public override bool Output(Processor processor, ActionFrame frame) => 
            processor.EndEvent(this.nodeType);
    }
}

