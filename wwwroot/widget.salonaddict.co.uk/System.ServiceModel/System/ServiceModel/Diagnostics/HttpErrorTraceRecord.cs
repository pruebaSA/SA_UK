namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Xml;

    internal class HttpErrorTraceRecord : TraceRecord
    {
        private string html;

        internal HttpErrorTraceRecord(string html)
        {
            this.html = DiagnosticTrace.XmlEncode(html);
        }

        internal override void WriteTo(XmlWriter writer)
        {
            writer.WriteElementString("HtmlErrorMessage", this.html);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/HttpErrorTraceRecord";
    }
}

