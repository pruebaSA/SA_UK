namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    [DataContract(Name="VolatileEnlistmentInDoubt")]
    internal abstract class VolatileEnlistmentInDoubtRecordSchema : TraceRecord
    {
        [DataMember(Name="EnlistmentId", IsRequired=true)]
        private Guid enlistmentId;
        protected string schemaId;

        protected VolatileEnlistmentInDoubtRecordSchema(Guid enlistmentId)
        {
            this.enlistmentId = enlistmentId;
        }

        public static VolatileEnlistmentInDoubtRecordSchema Instance(Guid enlistmentId, EndpointAddress replyTo, ProtocolVersion protocolVersion)
        {
            ProtocolVersionHelper.AssertProtocolVersion(protocolVersion, typeof(VolatileEnlistmentInDoubtRecordSchema), "Instance");
            switch (protocolVersion)
            {
                case ProtocolVersion.Version10:
                    return new VolatileEnlistmentInDoubtRecordSchema10(enlistmentId, replyTo);

                case ProtocolVersion.Version11:
                    return new VolatileEnlistmentInDoubtRecordSchema11(enlistmentId, replyTo);
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

