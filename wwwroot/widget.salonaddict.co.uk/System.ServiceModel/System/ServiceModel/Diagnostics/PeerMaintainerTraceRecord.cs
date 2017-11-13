namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Xml;

    internal class PeerMaintainerTraceRecord : TraceRecord
    {
        private string activity;

        public PeerMaintainerTraceRecord(string activity)
        {
            this.activity = activity;
        }

        internal override void WriteTo(XmlWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteElementString("Activity", this.activity);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/PeerMaintainerActivityTraceRecord";
    }
}

