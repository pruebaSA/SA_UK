namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="PerformanceCounter")]
    internal class PerformanceCounterSchema : TraceRecord
    {
        [DataMember(Name="Name", IsRequired=true)]
        private string counterName;
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/PerformanceCounterTraceRecord";

        public PerformanceCounterSchema(string counterName)
        {
            this.counterName = counterName;
        }

        public override string ToString() => 
            Microsoft.Transactions.SR.GetString("PerformanceCounterSchema", new object[] { this.counterName });

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/PerformanceCounterTraceRecord";
    }
}

