namespace System.Windows.Threading
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct DispatcherProcessingDisabled : IDisposable
    {
        internal Dispatcher _dispatcher;
        public void Dispose()
        {
            if (this._dispatcher != null)
            {
                this._dispatcher.VerifyAccess();
                this._dispatcher._disableProcessingCount--;
                this._dispatcher = null;
            }
        }

        public override bool Equals(object obj) => 
            (((obj != null) && (obj is DispatcherProcessingDisabled)) && (this._dispatcher == ((DispatcherProcessingDisabled) obj)._dispatcher));

        public override int GetHashCode() => 
            base.GetHashCode();

        public static bool operator ==(DispatcherProcessingDisabled left, DispatcherProcessingDisabled right) => 
            left.Equals(right);

        public static bool operator !=(DispatcherProcessingDisabled left, DispatcherProcessingDisabled right) => 
            !left.Equals(right);
    }
}

