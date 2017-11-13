namespace System.ServiceModel
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal abstract class AsyncResult : IAsyncResult
    {
        private AsyncCallback callback;
        private ServiceModelActivity callbackActivity;
        private bool completedSynchronously;
        private bool endCalled;
        private Exception exception;
        private bool isCompleted;
        private ManualResetEvent manualResetEvent;
        private object state;
        private object thisLock;

        protected AsyncResult(AsyncCallback callback, object state)
        {
            this.callback = callback;
            this.state = state;
            this.thisLock = new object();
        }

        protected void Complete(bool completedSynchronously)
        {
            if (this.isCompleted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            this.completedSynchronously = completedSynchronously;
            if (completedSynchronously)
            {
                this.isCompleted = true;
            }
            else
            {
                lock (this.ThisLock)
                {
                    this.isCompleted = true;
                    if (this.manualResetEvent != null)
                    {
                        this.manualResetEvent.Set();
                    }
                }
            }
            if (this.callback != null)
            {
                try
                {
                    using ((this.CallbackActivity == null) ? null : ServiceModelActivity.BoundOperation(this.CallbackActivity))
                    {
                        this.callback(this);
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.AsyncCallbackThrewException, exception, null);
                    }
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(System.ServiceModel.SR.GetString("AsyncCallbackException"), exception);
                }
            }
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            this.exception = exception;
            this.Complete(completedSynchronously);
        }

        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result) where TAsyncResult: AsyncResult
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("result");
            }
            TAsyncResult local = result as TAsyncResult;
            if (local == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("result", System.ServiceModel.SR.GetString("InvalidAsyncResult"));
            }
            if (local.endCalled)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("AsyncObjectAlreadyEnded")));
            }
            local.endCalled = true;
            if (!local.isCompleted)
            {
                local.AsyncWaitHandle.WaitOne();
            }
            if (local.manualResetEvent != null)
            {
                local.manualResetEvent.Close();
            }
            if (local.exception != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(local.exception);
            }
            return local;
        }

        public object AsyncState =>
            this.state;

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (this.manualResetEvent == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.manualResetEvent == null)
                        {
                            this.manualResetEvent = new ManualResetEvent(this.isCompleted);
                        }
                    }
                }
                return this.manualResetEvent;
            }
        }

        public ServiceModelActivity CallbackActivity
        {
            get => 
                this.callbackActivity;
            set
            {
                this.callbackActivity = value;
            }
        }

        public bool CompletedSynchronously =>
            this.completedSynchronously;

        public bool HasCallback =>
            (this.callback != null);

        public bool IsCompleted =>
            this.isCompleted;

        private object ThisLock =>
            this.thisLock;
    }
}

