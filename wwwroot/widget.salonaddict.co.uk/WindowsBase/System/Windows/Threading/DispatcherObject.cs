namespace System.Windows.Threading
{
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;

    public abstract class DispatcherObject
    {
        private System.Windows.Threading.Dispatcher _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

        protected DispatcherObject()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool CheckAccess()
        {
            bool flag = true;
            System.Windows.Threading.Dispatcher dispatcher = this._dispatcher;
            if (dispatcher != null)
            {
                flag = dispatcher.CheckAccess();
            }
            return flag;
        }

        [FriendAccessAllowed]
        internal void DetachFromDispatcher()
        {
            this._dispatcher = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void VerifyAccess()
        {
            System.Windows.Threading.Dispatcher dispatcher = this._dispatcher;
            if (dispatcher != null)
            {
                dispatcher.VerifyAccess();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public System.Windows.Threading.Dispatcher Dispatcher =>
            this._dispatcher;
    }
}

