namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PageAsyncTask
    {
        private IAsyncResult _asyncResult;
        private BeginEventHandler _beginHandler;
        private bool _completed;
        private bool _completedSynchronously;
        private AsyncCallback _completionCallback;
        private int _completionMethodLock;
        private EndEventHandler _endHandler;
        private Exception _error;
        private bool _executeInParallel;
        private bool _started;
        private object _state;
        private PageAsyncTaskManager _taskManager;
        private EndEventHandler _timeoutHandler;

        public PageAsyncTask(BeginEventHandler beginHandler, EndEventHandler endHandler, EndEventHandler timeoutHandler, object state) : this(beginHandler, endHandler, timeoutHandler, state, false)
        {
        }

        public PageAsyncTask(BeginEventHandler beginHandler, EndEventHandler endHandler, EndEventHandler timeoutHandler, object state, bool executeInParallel)
        {
            if (beginHandler == null)
            {
                throw new ArgumentNullException("beginHandler");
            }
            if (endHandler == null)
            {
                throw new ArgumentNullException("endHandler");
            }
            this._beginHandler = beginHandler;
            this._endHandler = endHandler;
            this._timeoutHandler = timeoutHandler;
            this._state = state;
            this._executeInParallel = executeInParallel;
        }

        private void CompleteTask(bool timedOut)
        {
            this.CompleteTask(timedOut, false);
        }

        private void CompleteTask(bool timedOut, bool syncTimeoutCaller)
        {
            bool flag;
            if (Interlocked.Exchange(ref this._completionMethodLock, 1) != 0)
            {
                return;
            }
            bool flag2 = false;
            if (timedOut)
            {
                flag = !syncTimeoutCaller;
            }
            else
            {
                this._completedSynchronously = this._asyncResult.CompletedSynchronously;
                flag = !this._completedSynchronously;
            }
            HttpApplication application = this._taskManager.Application;
            try
            {
                if (flag)
                {
                    lock (application)
                    {
                        HttpApplication.ThreadContext context = null;
                        try
                        {
                            context = application.OnThreadEnter();
                            if (timedOut)
                            {
                                if (this._timeoutHandler != null)
                                {
                                    this._timeoutHandler(this._asyncResult);
                                }
                            }
                            else
                            {
                                this._endHandler(this._asyncResult);
                            }
                        }
                        finally
                        {
                            if (context != null)
                            {
                                context.Leave();
                            }
                        }
                        goto Label_012F;
                    }
                }
                if (timedOut)
                {
                    if (this._timeoutHandler != null)
                    {
                        this._timeoutHandler(this._asyncResult);
                    }
                }
                else
                {
                    this._endHandler(this._asyncResult);
                }
            }
            catch (ThreadAbortException exception)
            {
                this._error = exception;
                HttpApplication.CancelModuleException exceptionState = exception.ExceptionState as HttpApplication.CancelModuleException;
                if ((exceptionState != null) && !exceptionState.Timeout)
                {
                    lock (application)
                    {
                        if (!application.IsRequestCompleted)
                        {
                            flag2 = true;
                            application.CompleteRequest();
                        }
                    }
                    this._error = null;
                }
                Thread.ResetAbort();
            }
            catch (Exception exception3)
            {
                this._error = exception3;
            }
        Label_012F:
            this._completed = true;
            this._taskManager.TaskCompleted(this._completedSynchronously);
            if (flag2)
            {
                this._taskManager.CompleteAllTasksNow(false);
            }
        }

        internal void ForceTimeout(bool syncCaller)
        {
            this.CompleteTask(true, syncCaller);
        }

        private void OnAsyncTaskCompletion(IAsyncResult ar)
        {
            if (this._asyncResult == null)
            {
                this._asyncResult = ar;
            }
            this.CompleteTask(false);
        }

        internal void Start(PageAsyncTaskManager manager, object source, EventArgs args)
        {
            this._taskManager = manager;
            this._completionCallback = new AsyncCallback(this.OnAsyncTaskCompletion);
            this._started = true;
            try
            {
                IAsyncResult result = this._beginHandler(source, args, this._completionCallback, this._state);
                if (result == null)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Async_null_asyncresult"));
                }
                if (this._asyncResult == null)
                {
                    this._asyncResult = result;
                }
            }
            catch (Exception exception)
            {
                this._error = exception;
                this._completed = true;
                this._completedSynchronously = true;
                this._taskManager.TaskCompleted(true);
            }
        }

        internal IAsyncResult AsyncResult =>
            this._asyncResult;

        public BeginEventHandler BeginHandler =>
            this._beginHandler;

        internal bool Completed =>
            this._completed;

        internal bool CompletedSynchronously =>
            this._completedSynchronously;

        public EndEventHandler EndHandler =>
            this._endHandler;

        internal Exception Error =>
            this._error;

        public bool ExecuteInParallel =>
            this._executeInParallel;

        internal bool Started =>
            this._started;

        public object State =>
            this._state;

        public EndEventHandler TimeoutHandler =>
            this._timeoutHandler;
    }
}

