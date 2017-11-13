namespace System.Net.NetworkInformation
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class SystemTcpStatistics : TcpStatistics
    {
        private MibTcpStats stats;

        private SystemTcpStatistics()
        {
        }

        internal SystemTcpStatistics(AddressFamily family)
        {
            uint tcpStatistics;
            if (!ComNetOS.IsPostWin2K)
            {
                if (family != AddressFamily.InterNetwork)
                {
                    throw new PlatformNotSupportedException(SR.GetString("WinXPRequired"));
                }
                tcpStatistics = UnsafeNetInfoNativeMethods.GetTcpStatistics(out this.stats);
            }
            else
            {
                tcpStatistics = UnsafeNetInfoNativeMethods.GetTcpStatisticsEx(out this.stats, family);
            }
            if (tcpStatistics != 0)
            {
                throw new NetworkInformationException((int) tcpStatistics);
            }
        }

        public override long ConnectionsAccepted =>
            ((long) this.stats.passiveOpens);

        public override long ConnectionsInitiated =>
            ((long) this.stats.activeOpens);

        public override long CumulativeConnections =>
            ((long) this.stats.cumulativeConnections);

        public override long CurrentConnections =>
            ((long) this.stats.currentConnections);

        public override long ErrorsReceived =>
            ((long) this.stats.errorsReceived);

        public override long FailedConnectionAttempts =>
            ((long) this.stats.failedConnectionAttempts);

        public override long MaximumConnections =>
            ((long) this.stats.maximumConnections);

        public override long MaximumTransmissionTimeout =>
            ((long) this.stats.maximumRetransmissionTimeOut);

        public override long MinimumTransmissionTimeout =>
            ((long) this.stats.minimumRetransmissionTimeOut);

        public override long ResetConnections =>
            ((long) this.stats.resetConnections);

        public override long ResetsSent =>
            ((long) this.stats.segmentsSentWithReset);

        public override long SegmentsReceived =>
            ((long) this.stats.segmentsReceived);

        public override long SegmentsResent =>
            ((long) this.stats.segmentsResent);

        public override long SegmentsSent =>
            ((long) this.stats.segmentsSent);
    }
}

