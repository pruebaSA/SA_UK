namespace System.DirectoryServices.Protocols
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Threading;

    internal class DsmlAsyncResult : IAsyncResult
    {
        private DsmlAsyncWaitHandle asyncWaitHandle;
        internal AsyncCallback callback;
        internal bool completed;
        private bool completedSynchronously;
        internal bool hasValidRequest;
        internal ManualResetEvent manualResetEvent;
        internal RequestState resultObject;
        private object stateObject;

        public DsmlAsyncResult(AsyncCallback callbackRoutine, object state)
        {
            this.stateObject = state;
            this.callback = callbackRoutine;
            this.manualResetEvent = new ManualResetEvent(false);
        }

        public override bool Equals(object o) => 
            (((o is DsmlAsyncResult) && (o != null)) && (this == ((DsmlAsyncResult) o)));

        public override int GetHashCode() => 
            this.manualResetEvent.GetHashCode();

        object IAsyncResult.AsyncState =>
            this.stateObject;

        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get
            {
                if (this.asyncWaitHandle == null)
                {
                    this.asyncWaitHandle = new DsmlAsyncWaitHandle(this.manualResetEvent.SafeWaitHandle);
                }
                return this.asyncWaitHandle;
            }
        }

        bool IAsyncResult.CompletedSynchronously =>
            this.completedSynchronously;

        bool IAsyncResult.IsCompleted =>
            this.completed;

        internal sealed class DsmlAsyncWaitHandle : WaitHandle
        {
            public DsmlAsyncWaitHandle(SafeWaitHandle handle)
            {
                base.SafeWaitHandle = handle;
            }

            ~DsmlAsyncWaitHandle()
            {
                base.SafeWaitHandle = null;
            }
        }
    }
}

