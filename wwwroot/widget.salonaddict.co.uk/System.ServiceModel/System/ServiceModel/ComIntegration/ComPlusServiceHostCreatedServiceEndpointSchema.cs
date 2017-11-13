namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;

    [DataContract(Name="ComPlusServiceHostCreatedServiceEndpoint")]
    internal class ComPlusServiceHostCreatedServiceEndpointSchema : ComPlusServiceHostSchema
    {
        [DataMember(Name="Address")]
        private Uri address;
        [DataMember(Name="Binding")]
        private string binding;
        [DataMember(Name="Contract")]
        private string contract;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusServiceHostCreatedServiceEndpointTraceRecord";

        public ComPlusServiceHostCreatedServiceEndpointSchema(Guid appid, Guid clsid, string contract, Uri address, string binding) : base(appid, clsid)
        {
            this.contract = contract;
            this.address = address;
            this.binding = binding;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            ComPlusTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusServiceHostCreatedServiceEndpointTraceRecord";
    }
}

