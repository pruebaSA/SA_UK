namespace System.Net.NetworkInformation
{
    using System;

    internal class SystemIcmpV4Statistics : IcmpV4Statistics
    {
        private MibIcmpInfo stats;

        internal SystemIcmpV4Statistics()
        {
            uint icmpStatistics = UnsafeNetInfoNativeMethods.GetIcmpStatistics(out this.stats);
            if (icmpStatistics != 0)
            {
                throw new NetworkInformationException((int) icmpStatistics);
            }
        }

        public override long AddressMaskRepliesReceived =>
            ((long) this.stats.inStats.addressMaskReplies);

        public override long AddressMaskRepliesSent =>
            ((long) this.stats.outStats.addressMaskReplies);

        public override long AddressMaskRequestsReceived =>
            ((long) this.stats.inStats.addressMaskRequests);

        public override long AddressMaskRequestsSent =>
            ((long) this.stats.outStats.addressMaskRequests);

        public override long DestinationUnreachableMessagesReceived =>
            ((long) this.stats.inStats.destinationUnreachables);

        public override long DestinationUnreachableMessagesSent =>
            ((long) this.stats.outStats.destinationUnreachables);

        public override long EchoRepliesReceived =>
            ((long) this.stats.inStats.echoReplies);

        public override long EchoRepliesSent =>
            ((long) this.stats.outStats.echoReplies);

        public override long EchoRequestsReceived =>
            ((long) this.stats.inStats.echoRequests);

        public override long EchoRequestsSent =>
            ((long) this.stats.outStats.echoRequests);

        public override long ErrorsReceived =>
            ((long) this.stats.inStats.errors);

        public override long ErrorsSent =>
            ((long) this.stats.outStats.errors);

        public override long MessagesReceived =>
            ((long) this.stats.inStats.messages);

        public override long MessagesSent =>
            ((long) this.stats.outStats.messages);

        public override long ParameterProblemsReceived =>
            ((long) this.stats.inStats.parameterProblems);

        public override long ParameterProblemsSent =>
            ((long) this.stats.outStats.parameterProblems);

        public override long RedirectsReceived =>
            ((long) this.stats.inStats.redirects);

        public override long RedirectsSent =>
            ((long) this.stats.outStats.redirects);

        public override long SourceQuenchesReceived =>
            ((long) this.stats.inStats.sourceQuenches);

        public override long SourceQuenchesSent =>
            ((long) this.stats.outStats.sourceQuenches);

        public override long TimeExceededMessagesReceived =>
            ((long) this.stats.inStats.timeExceeds);

        public override long TimeExceededMessagesSent =>
            ((long) this.stats.outStats.timeExceeds);

        public override long TimestampRepliesReceived =>
            ((long) this.stats.inStats.timestampReplies);

        public override long TimestampRepliesSent =>
            ((long) this.stats.outStats.timestampReplies);

        public override long TimestampRequestsReceived =>
            ((long) this.stats.inStats.timestampRequests);

        public override long TimestampRequestsSent =>
            ((long) this.stats.outStats.timestampRequests);
    }
}

