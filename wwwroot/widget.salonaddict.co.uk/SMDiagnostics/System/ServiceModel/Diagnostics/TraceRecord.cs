namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Xml;

    [Serializable]
    internal class TraceRecord
    {
        protected const string EventIdBase = "http://schemas.microsoft.com/2006/08/ServiceModel/";
        protected const string NamespaceSuffix = "TraceRecord";

        internal virtual void WriteTo(XmlWriter writer)
        {
        }

        internal virtual string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/EmptyTraceRecord";
    }
}

