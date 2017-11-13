namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class RtfTreeNavigator : RtfNavigator
    {
        private NavigatorConstructor constr;
        private XmlEventCache events;
        private XmlNameTable nameTable;

        public RtfTreeNavigator(RtfTreeNavigator that)
        {
            this.events = that.events;
            this.constr = that.constr;
            this.nameTable = that.nameTable;
        }

        public RtfTreeNavigator(XmlEventCache events, XmlNameTable nameTable)
        {
            this.events = events;
            this.constr = new NavigatorConstructor();
            this.nameTable = nameTable;
        }

        public override XPathNavigator Clone() => 
            new RtfTreeNavigator(this);

        public override void CopyToWriter(XmlWriter writer)
        {
            this.events.EventsToWriter(writer);
        }

        public override bool MoveTo(XPathNavigator other)
        {
            RtfTreeNavigator navigator = other as RtfTreeNavigator;
            if (navigator != null)
            {
                this.events = navigator.events;
                this.constr = navigator.constr;
                this.nameTable = navigator.nameTable;
                return true;
            }
            return false;
        }

        public override XPathNavigator ToNavigator() => 
            this.constr.GetNavigator(this.events, this.nameTable);

        public override string BaseURI =>
            this.events.BaseUri;

        public override string Value =>
            this.events.EventsToString();
    }
}

