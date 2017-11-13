﻿namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Net;
    using System.Xml;

    internal class HttpListenerRequestTraceRecord : TraceRecord
    {
        private HttpListenerRequest request;

        internal HttpListenerRequestTraceRecord(HttpListenerRequest request)
        {
            this.request = request;
        }

        internal override void WriteTo(XmlWriter writer)
        {
            writer.WriteStartElement("Headers");
            foreach (string str in this.request.Headers.Keys)
            {
                writer.WriteElementString(str, this.request.Headers[str]);
            }
            writer.WriteEndElement();
            writer.WriteElementString("Url", this.request.Url.ToString());
            if ((this.request.QueryString != null) && (this.request.QueryString.Count > 0))
            {
                writer.WriteStartElement("QueryString");
                foreach (string str2 in this.request.QueryString.Keys)
                {
                    writer.WriteElementString(str2, this.request.Headers[str2]);
                }
                writer.WriteEndElement();
            }
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/HttpRequestTraceRecord";
    }
}

