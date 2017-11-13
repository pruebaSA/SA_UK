namespace System.Web.Services.Protocols
{
    using System;
    using System.Threading;

    internal class CompletedAsyncResult : IAsyncResult
    {
        private object asyncState;
        private bool completedSynchronously;

        internal CompletedAsyncResult(object asyncState, bool completedSynchronously)
        {
            this.asyncState = asyncState;
            this.completedSynchronously = completedSynchronously;
        }

        public object AsyncState =>
            this.asyncState;

        public WaitHandle AsyncWaitHandle =>
            null;

        public bool CompletedSynchronously =>
            this.completedSynchronously;

        public bool IsCompleted =>
            true;
    }
}

