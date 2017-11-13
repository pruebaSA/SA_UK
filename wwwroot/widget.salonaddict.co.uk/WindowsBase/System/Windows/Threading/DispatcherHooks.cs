namespace System.Windows.Threading
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    public sealed class DispatcherHooks
    {
        [SecurityCritical]
        private EventHandler _dispatcherInactive;
        private object _instanceLock = new object();
        [SecurityCritical]
        private DispatcherHookEventHandler _operationAborted;
        [SecurityCritical]
        private DispatcherHookEventHandler _operationCompleted;
        [SecurityCritical]
        private DispatcherHookEventHandler _operationPosted;
        [SecurityCritical]
        private DispatcherHookEventHandler _operationPriorityChanged;

        public event EventHandler DispatcherInactive
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                lock (this._instanceLock)
                {
                    this._dispatcherInactive = (EventHandler) Delegate.Combine(this._dispatcherInactive, value);
                }
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                lock (this._instanceLock)
                {
                    this._dispatcherInactive = (EventHandler) Delegate.Remove(this._dispatcherInactive, value);
                }
            }
        }

        public event DispatcherHookEventHandler OperationAborted
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                lock (this._instanceLock)
                {
                    this._operationAborted = (DispatcherHookEventHandler) Delegate.Combine(this._operationAborted, value);
                }
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                lock (this._instanceLock)
                {
                    this._operationAborted = (DispatcherHookEventHandler) Delegate.Remove(this._operationAborted, value);
                }
            }
        }

        public event DispatcherHookEventHandler OperationCompleted
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                lock (this._instanceLock)
                {
                    this._operationCompleted = (DispatcherHookEventHandler) Delegate.Combine(this._operationCompleted, value);
                }
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                lock (this._instanceLock)
                {
                    this._operationCompleted = (DispatcherHookEventHandler) Delegate.Remove(this._operationCompleted, value);
                }
            }
        }

        public event DispatcherHookEventHandler OperationPosted
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                lock (this._instanceLock)
                {
                    this._operationPosted = (DispatcherHookEventHandler) Delegate.Combine(this._operationPosted, value);
                }
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                lock (this._instanceLock)
                {
                    this._operationPosted = (DispatcherHookEventHandler) Delegate.Remove(this._operationPosted, value);
                }
            }
        }

        public event DispatcherHookEventHandler OperationPriorityChanged
        {
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] add
            {
                lock (this._instanceLock)
                {
                    this._operationPriorityChanged = (DispatcherHookEventHandler) Delegate.Combine(this._operationPriorityChanged, value);
                }
            }
            [SecurityCritical, UIPermission(SecurityAction.LinkDemand, Unrestricted=true)] remove
            {
                lock (this._instanceLock)
                {
                    this._operationPriorityChanged = (DispatcherHookEventHandler) Delegate.Remove(this._operationPriorityChanged, value);
                }
            }
        }

        internal DispatcherHooks()
        {
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal void RaiseDispatcherInactive(Dispatcher dispatcher)
        {
            EventHandler handler = this._dispatcherInactive;
            if (handler != null)
            {
                handler(dispatcher, EventArgs.Empty);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal void RaiseOperationAborted(Dispatcher dispatcher, DispatcherOperation operation)
        {
            DispatcherHookEventHandler handler = this._operationAborted;
            if (handler != null)
            {
                handler(dispatcher, new DispatcherHookEventArgs(operation));
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal void RaiseOperationCompleted(Dispatcher dispatcher, DispatcherOperation operation)
        {
            DispatcherHookEventHandler handler = this._operationCompleted;
            if (handler != null)
            {
                handler(dispatcher, new DispatcherHookEventArgs(operation));
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void RaiseOperationPosted(Dispatcher dispatcher, DispatcherOperation operation)
        {
            DispatcherHookEventHandler handler = this._operationPosted;
            if (handler != null)
            {
                handler(dispatcher, new DispatcherHookEventArgs(operation));
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void RaiseOperationPriorityChanged(Dispatcher dispatcher, DispatcherOperation operation)
        {
            DispatcherHookEventHandler handler = this._operationPriorityChanged;
            if (handler != null)
            {
                handler(dispatcher, new DispatcherHookEventArgs(operation));
            }
        }
    }
}

