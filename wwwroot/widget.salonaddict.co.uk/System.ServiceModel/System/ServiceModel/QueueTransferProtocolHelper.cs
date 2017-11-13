namespace System.ServiceModel
{
    using System;

    internal static class QueueTransferProtocolHelper
    {
        public static bool IsDefined(QueueTransferProtocol mode) => 
            ((mode >= QueueTransferProtocol.Native) && (mode <= QueueTransferProtocol.SrmpSecure));
    }
}

