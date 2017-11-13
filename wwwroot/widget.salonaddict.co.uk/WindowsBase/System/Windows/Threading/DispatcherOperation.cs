namespace System.Windows.Threading
{
    using System;
    using System.Security;
    using System.Threading;
    using System.Windows;

    public sealed class DispatcherOperation
    {
        private EventHandler _aborted;
        private object _args;
        private EventHandler _completed;
        private System.Windows.Threading.Dispatcher _dispatcher;
        [SecurityCritical]
        private ExecutionContext _executionContext;
        private static ContextCallback _invokeInSecurityContext = new ContextCallback(DispatcherOperation.InvokeInSecurityContext);
        private bool _isSingleParameter;
        internal PriorityItem<DispatcherOperation> _item;
        private Delegate _method;
        private DispatcherPriority _priority;
        private object _result;
        internal DispatcherOperationStatus _status;

        public event EventHandler Aborted
        {
            add
            {
                lock (this.DispatcherLock)
                {
                    this._aborted = (EventHandler) Delegate.Combine(this._aborted, value);
                }
            }
            remove
            {
                lock (this.DispatcherLock)
                {
                    this._aborted = (EventHandler) Delegate.Remove(this._aborted, value);
                }
            }
        }

        public event EventHandler Completed
        {
            add
            {
                lock (this.DispatcherLock)
                {
                    this._completed = (EventHandler) Delegate.Combine(this._completed, value);
                }
            }
            remove
            {
                lock (this.DispatcherLock)
                {
                    this._completed = (EventHandler) Delegate.Remove(this._completed, value);
                }
            }
        }

        internal DispatcherOperation(System.Windows.Threading.Dispatcher dispatcher, Delegate method, DispatcherPriority priority)
        {
            this._dispatcher = dispatcher;
            this._method = method;
            this._priority = priority;
            this._status = DispatcherOperationStatus.Aborted;
        }

        [SecurityCritical]
        internal DispatcherOperation(System.Windows.Threading.Dispatcher dispatcher, Delegate method, DispatcherPriority priority, object args, bool isSingleParameter)
        {
            this._dispatcher = dispatcher;
            this._method = method;
            this._priority = priority;
            this._isSingleParameter = isSingleParameter;
            this._args = args;
            this._executionContext = ExecutionContext.Capture();
        }

        public bool Abort()
        {
            bool flag = false;
            if (this._dispatcher != null)
            {
                flag = this._dispatcher.Abort(this);
                if (flag)
                {
                    EventHandler handler = this._aborted;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
            return flag;
        }

        [SecurityCritical]
        internal object Invoke()
        {
            EventHandler handler;
            this._status = DispatcherOperationStatus.Executing;
            if (this._executionContext != null)
            {
                ExecutionContext.Run(this._executionContext, _invokeInSecurityContext, this);
            }
            else
            {
                _invokeInSecurityContext(this);
            }
            lock (this.DispatcherLock)
            {
                this._status = DispatcherOperationStatus.Completed;
                handler = this._completed;
            }
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            return this._result;
        }

        [SecurityCritical]
        private void InvokeImpl()
        {
            SynchronizationContext current = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(this._dispatcher));
                this._result = this._dispatcher.WrappedInvoke(this._method, this._args, this._isSingleParameter);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(current);
            }
        }

        [SecurityCritical]
        private static void InvokeInSecurityContext(object state)
        {
            ((DispatcherOperation) state).InvokeImpl();
        }

        public DispatcherOperationStatus Wait() => 
            this.Wait(TimeSpan.FromMilliseconds(-1.0));

        [SecurityCritical]
        public DispatcherOperationStatus Wait(TimeSpan timeout)
        {
            if (((this._status == DispatcherOperationStatus.Pending) || (this._status == DispatcherOperationStatus.Executing)) && (timeout.TotalMilliseconds != 0.0))
            {
                if (this._dispatcher.Thread == Thread.CurrentThread)
                {
                    if (this._status == DispatcherOperationStatus.Executing)
                    {
                        throw new InvalidOperationException(System.Windows.SR.Get("ThreadMayNotWaitOnOperationsAlreadyExecutingOnTheSameThread"));
                    }
                    DispatcherOperationFrame frame = new DispatcherOperationFrame(this, timeout);
                    System.Windows.Threading.Dispatcher.PushFrame(frame);
                }
                else
                {
                    new DispatcherOperationEvent(this, timeout).WaitOne();
                }
            }
            return this._status;
        }

        public System.Windows.Threading.Dispatcher Dispatcher =>
            this._dispatcher;

        private object DispatcherLock =>
            this._dispatcher._instanceLock;

        internal string Name =>
            (this._method.Method.DeclaringType + "." + this._method.Method.Name);

        public DispatcherPriority Priority
        {
            get => 
                this._priority;
            set
            {
                System.Windows.Threading.Dispatcher.ValidatePriority(value, "value");
                if ((value != this._priority) && this._dispatcher.SetPriority(this, value))
                {
                    this._priority = value;
                }
            }
        }

        public object Result =>
            this._result;

        public DispatcherOperationStatus Status =>
            this._status;

        private class DispatcherOperationEvent
        {
            private ManualResetEvent _event;
            private bool _eventClosed;
            private DispatcherOperation _operation;
            private TimeSpan _timeout;

            public DispatcherOperationEvent(DispatcherOperation op, TimeSpan timeout)
            {
                this._operation = op;
                this._timeout = timeout;
                this._event = new ManualResetEvent(false);
                this._eventClosed = false;
                lock (this.DispatcherLock)
                {
                    this._operation.Aborted += new EventHandler(this.OnCompletedOrAborted);
                    this._operation.Completed += new EventHandler(this.OnCompletedOrAborted);
                    if ((this._operation._status != DispatcherOperationStatus.Pending) && (this._operation._status != DispatcherOperationStatus.Executing))
                    {
                        this._event.Set();
                    }
                }
            }

            private void OnCompletedOrAborted(object sender, EventArgs e)
            {
                lock (this.DispatcherLock)
                {
                    if (!this._eventClosed)
                    {
                        this._event.Set();
                    }
                }
            }

            public void WaitOne()
            {
                this._event.WaitOne(this._timeout, false);
                lock (this.DispatcherLock)
                {
                    if (!this._eventClosed)
                    {
                        this._operation.Aborted -= new EventHandler(this.OnCompletedOrAborted);
                        this._operation.Completed -= new EventHandler(this.OnCompletedOrAborted);
                        this._event.Close();
                        this._eventClosed = true;
                    }
                }
            }

            private object DispatcherLock =>
                this._operation.DispatcherLock;
        }

        private class DispatcherOperationFrame : DispatcherFrame
        {
            private DispatcherOperation _operation;
            private Timer _waitTimer;

            public DispatcherOperationFrame(DispatcherOperation op, TimeSpan timeout) : base(false)
            {
                this._operation = op;
                this._operation.Aborted += new EventHandler(this.OnCompletedOrAborted);
                this._operation.Completed += new EventHandler(this.OnCompletedOrAborted);
                if (timeout.TotalMilliseconds > 0.0)
                {
                    this._waitTimer = new Timer(new TimerCallback(this.OnTimeout), null, timeout, TimeSpan.FromMilliseconds(-1.0));
                }
                if (this._operation._status != DispatcherOperationStatus.Pending)
                {
                    this.Exit();
                }
            }

            private void Exit()
            {
                base.Continue = false;
                if (this._waitTimer != null)
                {
                    this._waitTimer.Dispose();
                }
                this._operation.Aborted -= new EventHandler(this.OnCompletedOrAborted);
                this._operation.Completed -= new EventHandler(this.OnCompletedOrAborted);
            }

            private void OnCompletedOrAborted(object sender, EventArgs e)
            {
                this.Exit();
            }

            private void OnTimeout(object arg)
            {
                this.Exit();
            }
        }
    }
}

