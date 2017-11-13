namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal class ActivityIdHeader : DictionaryHeader
    {
        private Guid guid;
        private Guid headerId;

        internal ActivityIdHeader(Guid activityId)
        {
            this.guid = activityId;
            this.headerId = Guid.NewGuid();
        }

        internal void AddTo(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (((message.State != MessageState.Closed) && (message.Headers.MessageVersion.Envelope != EnvelopeVersion.None)) && (message.Headers.FindHeader("ActivityId", "http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics") < 0))
            {
                message.Headers.Add(this);
            }
        }

        internal static Guid ExtractActivityId(Message message)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            Guid empty = Guid.Empty;
            try
            {
                if ((message.State == MessageState.Closed) || (message.Headers == null))
                {
                    return empty;
                }
                int headerIndex = message.Headers.FindHeader("ActivityId", "http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics");
                if (headerIndex < 0)
                {
                    return empty;
                }
                using (XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(headerIndex))
                {
                    empty = reader.ReadElementContentAsGuid();
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (DiagnosticUtility.ShouldTraceError)
                {
                    TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.FailedToReadAnActivityIdHeader, null, exception);
                }
            }
            return empty;
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            writer.WriteAttributeString("CorrelationId", this.headerId.ToString());
            writer.WriteValue(this.guid);
        }

        public override XmlDictionaryString DictionaryName =>
            XD.ActivityIdFlowDictionary.ActivityId;

        public override XmlDictionaryString DictionaryNamespace =>
            XD.ActivityIdFlowDictionary.ActivityIdNamespace;
    }
}

