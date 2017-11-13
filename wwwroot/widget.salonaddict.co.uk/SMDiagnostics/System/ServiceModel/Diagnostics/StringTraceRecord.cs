namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Xml;

    internal class StringTraceRecord : TraceRecord
    {
        private string content;
        private string elementName;

        internal StringTraceRecord(string elementName, string content)
        {
            this.elementName = elementName;
            this.content = content;
        }

        internal override void WriteTo(XmlWriter writer)
        {
            writer.WriteElementString(this.elementName, this.content);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/StringTraceRecord";
    }
}

