namespace System.Net.NetworkInformation
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal class SystemIPGlobalStatistics : IPGlobalStatistics
    {
        private MibIpStats stats;

        private SystemIPGlobalStatistics()
        {
            this.stats = new MibIpStats();
        }

        internal SystemIPGlobalStatistics(AddressFamily family)
        {
            uint ipStatistics;
            this.stats = new MibIpStats();
            if (!ComNetOS.IsPostWin2K)
            {
                if (family != AddressFamily.InterNetwork)
                {
                    throw new PlatformNotSupportedException(SR.GetString("WinXPRequired"));
                }
                ipStatistics = UnsafeNetInfoNativeMethods.GetIpStatistics(out this.stats);
            }
            else
            {
                ipStatistics = UnsafeNetInfoNativeMethods.GetIpStatisticsEx(out this.stats, family);
            }
            if (ipStatistics != 0)
            {
                throw new NetworkInformationException((int) ipStatistics);
            }
        }

        public override int DefaultTtl =>
            ((int) this.stats.defaultTtl);

        public override bool ForwardingEnabled =>
            this.stats.forwardingEnabled;

        public override int NumberOfInterfaces =>
            ((int) this.stats.interfaces);

        public override int NumberOfIPAddresses =>
            ((int) this.stats.ipAddresses);

        public override int NumberOfRoutes =>
            ((int) this.stats.routes);

        public override long OutputPacketRequests =>
            ((long) this.stats.packetOutputRequests);

        public override long OutputPacketRoutingDiscards =>
            ((long) this.stats.outputPacketRoutingDiscards);

        public override long OutputPacketsDiscarded =>
            ((long) this.stats.outputPacketsDiscarded);

        public override long OutputPacketsWithNoRoute =>
            ((long) this.stats.outputPacketsWithNoRoute);

        public override long PacketFragmentFailures =>
            ((long) this.stats.packetsFragmentFailed);

        public override long PacketReassembliesRequired =>
            ((long) this.stats.packetsReassemblyRequired);

        public override long PacketReassemblyFailures =>
            ((long) this.stats.packetsReassemblyFailed);

        public override long PacketReassemblyTimeout =>
            ((long) this.stats.packetReassemblyTimeout);

        public override long PacketsFragmented =>
            ((long) this.stats.packetsFragmented);

        public override long PacketsReassembled =>
            ((long) this.stats.packetsReassembled);

        public override long ReceivedPackets =>
            ((long) this.stats.packetsReceived);

        public override long ReceivedPacketsDelivered =>
            ((long) this.stats.receivedPacketsDelivered);

        public override long ReceivedPacketsDiscarded =>
            ((long) this.stats.receivedPacketsDiscarded);

        public override long ReceivedPacketsForwarded =>
            ((long) this.stats.packetsForwarded);

        public override long ReceivedPacketsWithAddressErrors =>
            ((long) this.stats.receivedPacketsWithAddressErrors);

        public override long ReceivedPacketsWithHeadersErrors =>
            ((long) this.stats.receivedPacketsWithHeaderErrors);

        public override long ReceivedPacketsWithUnknownProtocol =>
            ((long) this.stats.receivedPacketsWithUnknownProtocols);
    }
}

