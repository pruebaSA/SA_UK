﻿namespace System.Net.NetworkInformation
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class SystemUdpStatistics : UdpStatistics
    {
        private MibUdpStats stats;

        private SystemUdpStatistics()
        {
        }

        internal SystemUdpStatistics(AddressFamily family)
        {
            uint udpStatistics;
            if (!ComNetOS.IsPostWin2K)
            {
                if (family != AddressFamily.InterNetwork)
                {
                    throw new PlatformNotSupportedException(SR.GetString("WinXPRequired"));
                }
                udpStatistics = UnsafeNetInfoNativeMethods.GetUdpStatistics(out this.stats);
            }
            else
            {
                udpStatistics = UnsafeNetInfoNativeMethods.GetUdpStatisticsEx(out this.stats, family);
            }
            if (udpStatistics != 0)
            {
                throw new NetworkInformationException((int) udpStatistics);
            }
        }

        public override long DatagramsReceived =>
            ((long) this.stats.datagramsReceived);

        public override long DatagramsSent =>
            ((long) this.stats.datagramsSent);

        public override long IncomingDatagramsDiscarded =>
            ((long) this.stats.incomingDatagramsDiscarded);

        public override long IncomingDatagramsWithErrors =>
            ((long) this.stats.incomingDatagramsWithErrors);

        public override int UdpListeners =>
            ((int) this.stats.udpListeners);
    }
}

