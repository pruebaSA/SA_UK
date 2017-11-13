namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="Transaction")]
    internal class TransactionRecordSchema : TraceRecord
    {
        private const string schemaId = "http://schemas.microsoft.com/2006/08/ServiceModel/TransactionTraceRecord";
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        public TransactionRecordSchema(string transactionId)
        {
            this.transactionId = transactionId;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/TransactionTraceRecord";
    }
}

