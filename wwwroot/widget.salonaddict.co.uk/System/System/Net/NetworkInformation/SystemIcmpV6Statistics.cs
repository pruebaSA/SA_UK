namespace System.Net.NetworkInformation
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class SystemIcmpV6Statistics : IcmpV6Statistics
    {
        private MibIcmpInfoEx stats;

        internal SystemIcmpV6Statistics()
        {
            if (!ComNetOS.IsPostWin2K)
            {
                throw new PlatformNotSupportedException(SR.GetString("WinXPRequired"));
            }
            uint icmpStatisticsEx = UnsafeNetInfoNativeMethods.GetIcmpStatisticsEx(out this.stats, AddressFamily.InterNetworkV6);
            if (icmpStatisticsEx != 0)
            {
                throw new NetworkInformationException((int) icmpStatisticsEx);
            }
        }

        public override long DestinationUnreachableMessagesReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 1L)]);

        public override long DestinationUnreachableMessagesSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 1L)]);

        public override long EchoRepliesReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x81L)]);

        public override long EchoRepliesSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x81L)]);

        public override long EchoRequestsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x80L)]);

        public override long EchoRequestsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x80L)]);

        public override long ErrorsReceived =>
            ((long) this.stats.inStats.dwErrors);

        public override long ErrorsSent =>
            ((long) this.stats.outStats.dwErrors);

        public override long MembershipQueriesReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 130L)]);

        public override long MembershipQueriesSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 130L)]);

        public override long MembershipReductionsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x84L)]);

        public override long MembershipReductionsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x84L)]);

        public override long MembershipReportsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x83L)]);

        public override long MembershipReportsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x83L)]);

        public override long MessagesReceived =>
            ((long) this.stats.inStats.dwMsgs);

        public override long MessagesSent =>
            ((long) this.stats.outStats.dwMsgs);

        public override long NeighborAdvertisementsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x88L)]);

        public override long NeighborAdvertisementsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x88L)]);

        public override long NeighborSolicitsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x87L)]);

        public override long NeighborSolicitsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x87L)]);

        public override long PacketTooBigMessagesReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 2L)]);

        public override long PacketTooBigMessagesSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 2L)]);

        public override long ParameterProblemsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 4L)]);

        public override long ParameterProblemsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 4L)]);

        public override long RedirectsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x89L)]);

        public override long RedirectsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x89L)]);

        public override long RouterAdvertisementsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x86L)]);

        public override long RouterAdvertisementsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x86L)]);

        public override long RouterSolicitsReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 0x85L)]);

        public override long RouterSolicitsSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 0x85L)]);

        public override long TimeExceededMessagesReceived =>
            ((long) this.stats.inStats.rgdwTypeCount[(int) ((IntPtr) 3L)]);

        public override long TimeExceededMessagesSent =>
            ((long) this.stats.outStats.rgdwTypeCount[(int) ((IntPtr) 3L)]);
    }
}

