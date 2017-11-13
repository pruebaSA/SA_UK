﻿namespace System.ServiceModel.Channels
{
    using System;

    internal static class PeerConnectorHelper
    {
        public static bool IsDefined(DisconnectReason value)
        {
            if (((value != DisconnectReason.LeavingMesh) && (value != DisconnectReason.NotUsefulNeighbor)) && ((value != DisconnectReason.DuplicateNeighbor) && (value != DisconnectReason.DuplicateNodeId)))
            {
                return (value == DisconnectReason.NodeBusy);
            }
            return true;
        }

        public static bool IsDefined(RefuseReason value)
        {
            if ((value != RefuseReason.DuplicateNodeId) && (value != RefuseReason.DuplicateNeighbor))
            {
                return (value == RefuseReason.NodeBusy);
            }
            return true;
        }
    }
}

