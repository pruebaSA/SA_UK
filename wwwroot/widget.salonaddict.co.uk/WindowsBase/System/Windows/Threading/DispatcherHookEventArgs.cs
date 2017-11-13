namespace System.Windows.Threading
{
    using System;

    public sealed class DispatcherHookEventArgs : EventArgs
    {
        private DispatcherOperation _operation;

        public DispatcherHookEventArgs(DispatcherOperation operation)
        {
            this._operation = operation;
        }

        public System.Windows.Threading.Dispatcher Dispatcher =>
            this._operation?.Dispatcher;

        public DispatcherOperation Operation =>
            this._operation;
    }
}

