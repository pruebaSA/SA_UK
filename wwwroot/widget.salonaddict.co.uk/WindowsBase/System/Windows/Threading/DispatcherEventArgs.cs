namespace System.Windows.Threading
{
    using System;

    public class DispatcherEventArgs : EventArgs
    {
        private System.Windows.Threading.Dispatcher _dispatcher;

        internal DispatcherEventArgs(System.Windows.Threading.Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }

        public System.Windows.Threading.Dispatcher Dispatcher =>
            this._dispatcher;
    }
}

