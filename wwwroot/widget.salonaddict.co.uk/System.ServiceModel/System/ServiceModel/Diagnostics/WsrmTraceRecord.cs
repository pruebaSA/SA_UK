namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Xml;

    internal class WsrmTraceRecord : TraceRecord
    {
        private UniqueId id;

        internal WsrmTraceRecord(UniqueId id)
        {
            this.id = id;
        }

        internal override void WriteTo(XmlWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteStartElement("Identifier");
            writer.WriteString(this.id.ToString());
            writer.WriteEndElement();
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/SequenceTraceRecord";
    }
}

