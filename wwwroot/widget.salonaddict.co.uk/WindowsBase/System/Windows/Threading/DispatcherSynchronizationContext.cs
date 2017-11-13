namespace System.Windows.Threading
{
    using MS.Win32;
    using System;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    public sealed class DispatcherSynchronizationContext : SynchronizationContext
    {
        internal Dispatcher _dispatcher;

        public DispatcherSynchronizationContext() : this(Dispatcher.CurrentDispatcher)
        {
        }

        public DispatcherSynchronizationContext(Dispatcher dispatcher)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }
            this._dispatcher = dispatcher;
            base.SetWaitNotificationRequired();
        }

        public override SynchronizationContext CreateCopy() => 
            new DispatcherSynchronizationContext(this._dispatcher);

        public override void Post(SendOrPostCallback d, object state)
        {
            this._dispatcher.BeginInvoke(DispatcherPriority.Normal, d, state);
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            this._dispatcher.Invoke(DispatcherPriority.Normal, d, state);
        }

        [SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlPolicy | SecurityPermissionFlag.ControlEvidence)]
        public override int Wait(IntPtr[] waitHandles, bool waitAll, int millisecondsTimeout)
        {
            if (this._dispatcher._disableProcessingCount > 0)
            {
                return MS.Win32.UnsafeNativeMethods.WaitForMultipleObjectsEx(waitHandles.Length, waitHandles, waitAll, millisecondsTimeout, false);
            }
            return SynchronizationContext.WaitHelper(waitHandles, waitAll, millisecondsTimeout);
        }
    }
}

