namespace System.Windows.Threading
{
    using System;

    public sealed class DispatcherUnhandledExceptionEventArgs : DispatcherEventArgs
    {
        private System.Exception _exception;
        private bool _handled;

        internal DispatcherUnhandledExceptionEventArgs(Dispatcher dispatcher) : base(dispatcher)
        {
        }

        internal void Initialize(System.Exception exception, bool handled)
        {
            this._exception = exception;
            this._handled = handled;
        }

        public System.Exception Exception =>
            this._exception;

        public bool Handled
        {
            get => 
                this._handled;
            set
            {
                if (value)
                {
                    this._handled = value;
                }
            }
        }
    }
}

