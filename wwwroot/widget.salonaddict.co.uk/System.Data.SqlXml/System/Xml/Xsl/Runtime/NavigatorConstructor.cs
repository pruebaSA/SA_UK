namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class NavigatorConstructor
    {
        private object cache;

        public XPathNavigator GetNavigator(XmlEventCache events, XmlNameTable nameTable)
        {
            if (this.cache == null)
            {
                XPathDocument document = new XPathDocument(nameTable);
                XmlRawWriter writer = document.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames | (events.HasRootNode ? XPathDocument.LoadFlags.None : XPathDocument.LoadFlags.Fragment), events.BaseUri);
                events.EventsToWriter(writer);
                writer.Close();
                this.cache = document;
            }
            return ((XPathDocument) this.cache).CreateNavigator();
        }

        public XPathNavigator GetNavigator(string text, string baseUri, XmlNameTable nameTable)
        {
            if (this.cache == null)
            {
                XPathDocument document = new XPathDocument(nameTable);
                XmlRawWriter writer = document.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, baseUri);
                writer.WriteString(text);
                writer.Close();
                this.cache = document;
            }
            return ((XPathDocument) this.cache).CreateNavigator();
        }
    }
}

