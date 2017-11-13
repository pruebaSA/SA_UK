namespace System.ServiceModel.Channels
{
    using System;

    internal enum DisconnectReason
    {
        DuplicateNeighbor = 4,
        DuplicateNodeId = 5,
        LeavingMesh = 2,
        NodeBusy = 6,
        NotUsefulNeighbor = 3
    }
}

