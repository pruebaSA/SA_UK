namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ComPlusDllHostInitializer")]
    internal class ComPlusDllHostInitializerSchema : TraceRecord
    {
        [DataMember(Name="appid")]
        private Guid appid;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusDllHostInitializerTraceRecord";

        public ComPlusDllHostInitializerSchema(Guid appid)
        {
            this.appid = appid;
        }

        public override string ToString() => 
            System.ServiceModel.SR.GetString("ComPlusServiceSchemaDllHost", new object[] { this.appid.ToString() });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            ComPlusTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusDllHostInitializerTraceRecord";
    }
}

