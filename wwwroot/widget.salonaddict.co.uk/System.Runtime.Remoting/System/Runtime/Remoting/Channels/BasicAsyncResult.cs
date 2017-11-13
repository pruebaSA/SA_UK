namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Threading;

    internal class BasicAsyncResult : IAsyncResult
    {
        private AsyncCallback _asyncCallback;
        private object _asyncState;
        private bool _bIsComplete;
        private System.Exception _exception;
        private ManualResetEvent _manualResetEvent;
        private object _returnValue;

        internal BasicAsyncResult(AsyncCallback callback, object state)
        {
            this._asyncCallback = callback;
            this._asyncState = state;
        }

        internal virtual void CleanupOnComplete()
        {
        }

        internal void SetComplete(object returnValue, System.Exception exception)
        {
            this._returnValue = returnValue;
            this._exception = exception;
            this.CleanupOnComplete();
            this._bIsComplete = true;
            try
            {
                if (this._manualResetEvent != null)
                {
                    this._manualResetEvent.Set();
                }
            }
            catch (System.Exception exception2)
            {
                if (this._exception == null)
                {
                    this._exception = exception2;
                }
            }
            catch
            {
                if (this._exception == null)
                {
                    this._exception = new System.Exception(CoreChannel.GetResourceString("Remoting_nonClsCompliantException"));
                }
            }
            if (this._asyncCallback != null)
            {
                this._asyncCallback(this);
            }
        }

        public object AsyncState =>
            this._asyncState;

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                bool initialState = this._bIsComplete;
                if (this._manualResetEvent == null)
                {
                    lock (this)
                    {
                        if (this._manualResetEvent == null)
                        {
                            this._manualResetEvent = new ManualResetEvent(initialState);
                        }
                    }
                }
                if (!initialState && this._bIsComplete)
                {
                    this._manualResetEvent.Set();
                }
                return this._manualResetEvent;
            }
        }

        public bool CompletedSynchronously =>
            false;

        internal System.Exception Exception =>
            this._exception;

        public bool IsCompleted =>
            this._bIsComplete;
    }
}

