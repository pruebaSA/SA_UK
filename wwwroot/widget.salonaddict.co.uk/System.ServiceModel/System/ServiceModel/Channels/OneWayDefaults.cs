namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal static class OneWayDefaults
    {
        internal const string IdleTimeoutString = "00:02:00";
        internal const string LeaseTimeoutString = "00:10:00";
        internal const int MaxAcceptedChannels = 10;
        internal const int MaxOutboundChannelsPerEndpoint = 10;
        internal const bool PacketRoutable = false;

        internal static TimeSpan IdleTimeout =>
            TimeSpanHelper.FromMinutes(2, "00:02:00");

        internal static TimeSpan LeaseTimeout =>
            TimeSpanHelper.FromMinutes(10, "00:10:00");
    }
}

