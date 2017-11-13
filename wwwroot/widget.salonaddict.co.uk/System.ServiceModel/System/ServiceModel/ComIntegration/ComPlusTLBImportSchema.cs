namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="ComPlusTLBImport")]
    internal class ComPlusTLBImportSchema : TraceRecord
    {
        [DataMember(Name="InterfaceID")]
        private Guid iid;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusTLBImportTraceRecord";
        [DataMember(Name="TypeLibraryID")]
        private Guid typeLibraryID;

        public ComPlusTLBImportSchema(Guid iid, Guid typeLibraryID)
        {
            this.iid = iid;
            this.typeLibraryID = typeLibraryID;
        }

        public override string ToString() => 
            System.ServiceModel.SR.GetString("ComPlusTLBImportSchema", new object[] { this.iid.ToString(), this.typeLibraryID.ToString() });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            ComPlusTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/ComPlusTLBImportTraceRecord";
    }
}

