namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ComPlusServiceHost")]
    internal class ComPlusServiceHostSchema : TraceRecord
    {
        [DataMember(Name="appid")]
        private Guid appid;
        [DataMember(Name="clsid")]
        private Guid clsid;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusServiceHostTraceRecord";

        public ComPlusServiceHostSchema(Guid appid, Guid clsid)
        {
            this.appid = appid;
            this.clsid = clsid;
        }

        public override string ToString() => 
            System.ServiceModel.SR.GetString("ComPlusServiceSchema", new object[] { this.appid.ToString(), this.clsid.ToString() });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            ComPlusTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusServiceHostTraceRecord";
    }
}

