namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MessageAttemptInfo
    {
        private readonly System.ServiceModel.Channels.Message message;
        private readonly int retryCount;
        private readonly long sequenceNumber;
        private readonly object state;
        public MessageAttemptInfo(System.ServiceModel.Channels.Message message, long sequenceNumber, int retryCount, object state)
        {
            this.message = message;
            this.sequenceNumber = sequenceNumber;
            this.retryCount = retryCount;
            this.state = state;
        }

        public System.ServiceModel.Channels.Message Message =>
            this.message;
        public int RetryCount =>
            this.retryCount;
        public object State =>
            this.state;
        public long GetSequenceNumber()
        {
            if (this.sequenceNumber <= 0L)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return this.sequenceNumber;
        }
    }
}

