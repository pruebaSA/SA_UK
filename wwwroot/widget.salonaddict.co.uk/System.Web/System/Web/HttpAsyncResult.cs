namespace System.Web
{
    using System;
    using System.Threading;

    internal class HttpAsyncResult : IAsyncResult
    {
        private object _asyncState;
        private AsyncCallback _callback;
        private bool _completed;
        private bool _completedSynchronously;
        private Exception _error;
        private object _result;
        private RequestNotificationStatus _status;

        internal HttpAsyncResult(AsyncCallback cb, object state)
        {
            this._callback = cb;
            this._asyncState = state;
            this._status = RequestNotificationStatus.Continue;
        }

        internal HttpAsyncResult(AsyncCallback cb, object state, bool completed, object result, Exception error)
        {
            this._callback = cb;
            this._asyncState = state;
            this._completed = completed;
            this._completedSynchronously = completed;
            this._result = result;
            this._error = error;
            this._status = RequestNotificationStatus.Continue;
            if (this._completed && (this._callback != null))
            {
                this._callback(this);
            }
        }

        internal void Complete(bool synchronous, object result, Exception error)
        {
            this.Complete(synchronous, result, error, RequestNotificationStatus.Continue);
        }

        internal void Complete(bool synchronous, object result, Exception error, RequestNotificationStatus status)
        {
            this._completed = true;
            this._completedSynchronously = synchronous;
            this._result = result;
            this._error = error;
            this._status = status;
            if (this._callback != null)
            {
                this._callback(this);
            }
        }

        internal object End()
        {
            if (this._error != null)
            {
                throw new HttpException(null, this._error);
            }
            return this._result;
        }

        internal void SetComplete()
        {
            this._completed = true;
        }

        public object AsyncState =>
            this._asyncState;

        public WaitHandle AsyncWaitHandle =>
            null;

        public bool CompletedSynchronously =>
            this._completedSynchronously;

        internal Exception Error =>
            this._error;

        public bool IsCompleted =>
            this._completed;

        internal RequestNotificationStatus Status =>
            this._status;
    }
}

