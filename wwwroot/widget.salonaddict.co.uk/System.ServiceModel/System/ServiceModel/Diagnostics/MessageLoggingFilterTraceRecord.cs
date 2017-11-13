namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.ServiceModel.Dispatcher;
    using System.Xml;

    internal class MessageLoggingFilterTraceRecord : TraceRecord
    {
        private XPathMessageFilter filter;

        internal MessageLoggingFilterTraceRecord(XPathMessageFilter filter)
        {
            this.filter = filter;
        }

        internal override void WriteTo(XmlWriter writer)
        {
            this.filter.WriteXPathTo(writer, "", "Filter", "", false);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/MessageLoggingFilterTraceRecord";
    }
}

