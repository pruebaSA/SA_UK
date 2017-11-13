namespace System.ServiceModel.Channels
{
    using System;

    internal class PeerNeighborCloseEventArgs : EventArgs
    {
        private PeerCloseInitiator closeInitiator;
        private System.Exception exception;
        private PeerCloseReason reason;

        public PeerNeighborCloseEventArgs(PeerCloseReason reason, PeerCloseInitiator closeInitiator, System.Exception exception)
        {
            this.reason = reason;
            this.closeInitiator = closeInitiator;
            this.exception = exception;
        }

        public PeerCloseInitiator CloseInitiator =>
            this.closeInitiator;

        public System.Exception Exception =>
            this.exception;

        public PeerCloseReason Reason =>
            this.reason;
    }
}

