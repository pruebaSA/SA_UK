namespace System.Windows.Threading
{
    using System;

    public sealed class DispatcherUnhandledExceptionFilterEventArgs : DispatcherEventArgs
    {
        private System.Exception _exception;
        private bool _requestCatch;

        internal DispatcherUnhandledExceptionFilterEventArgs(Dispatcher dispatcher) : base(dispatcher)
        {
        }

        internal void Initialize(System.Exception exception, bool requestCatch)
        {
            this._exception = exception;
            this._requestCatch = requestCatch;
        }

        public System.Exception Exception =>
            this._exception;

        public bool RequestCatch
        {
            get => 
                this._requestCatch;
            set
            {
                if (!value)
                {
                    this._requestCatch = value;
                }
            }
        }
    }
}

