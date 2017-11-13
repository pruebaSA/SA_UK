﻿namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="RegisterParticipant")]
    internal abstract class RegisterParticipantRecordSchema : TraceRecord
    {
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        private ControlProtocol protocol;
        protected string schemaId;
        [DataMember(Name="TransactionId", IsRequired=true)]
        private string transactionId;

        protected RegisterParticipantRecordSchema(string transactionId, Guid enlistmentId, ControlProtocol protocol)
        {
            this.transactionId = transactionId;
            this.enlistmentId = enlistmentId;
            this.protocol = protocol;
        }

        public static RegisterParticipantRecordSchema Instance(string transactionId, Guid enlistmentId, ControlProtocol protocol, EndpointAddress participantService, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(RegisterParticipantRecordSchema), "Instance");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new RegisterParticipantRecordSchema10(transactionId, enlistmentId, protocol, participantService);

                case ProtocolVersion.Version11:
                    return new RegisterParticipantRecordSchema11(transactionId, enlistmentId, protocol, participantService);
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

