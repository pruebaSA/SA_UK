﻿namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="RecoverCoordinator")]
    internal abstract class RecoverCoordinatorRecordSchema : TraceRecord
    {
        protected string schemaId;
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        protected RecoverCoordinatorRecordSchema(string transactionId)
        {
            this.transactionId = transactionId;
        }

        public static RecoverCoordinatorRecordSchema Instance(string transactionId, EndpointAddress coordinatorService, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(RecoverCoordinatorRecordSchema), "Instance");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new RecoverCoordinatorRecordSchema10(transactionId, coordinatorService);

                case ProtocolVersion.Version11:
                    return new RecoverCoordinatorRecordSchema11(transactionId, coordinatorService);
            }
            return null;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            this.schemaId;
    }
}

