﻿namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="RegisterFailure")]
    internal abstract class RegisterFailureRecordSchema : TraceRecord
    {
        private ControlProtocol protocol;
        [DataMember(Name="Reason", IsRequired=true)]
        private string reason;
        protected string schemaId;
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        protected RegisterFailureRecordSchema(string transactionId, ControlProtocol protocol, string reason)
        {
            this.transactionId = transactionId;
            this.protocol = protocol;
            this.reason = reason;
        }

        public static RegisterFailureRecordSchema Instance(string transactionId, ControlProtocol protocol, EndpointAddress protocolService, string reason, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(RegisterFailureRecordSchema), "Instance");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new RegisterFailureRecordSchema10(transactionId, protocol, protocolService, reason);

                case ProtocolVersion.Version11:
                    return new RegisterFailureRecordSchema11(transactionId, protocol, protocolService, reason);
            }
            return null;
        }

        internal override void WriteTo(XmlWriter xmlWriter)
        {
            TransactionTraceRecord.SerializeRecord(xmlWriter, this);
        }

        internal override string EventId =>
            this.schemaId;

        [DataMember(Name="Protocol", IsRequired=true)]
        private string Protocol
        {
            get => 
                this.protocol.ToString();
            set
            {
            }
        }
    }
}

