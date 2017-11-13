namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;
    using System.Windows.Threading;

    [FriendAccessAllowed]
    internal abstract class ShutDownListener : WeakReference
    {
        private WeakReference _dispatcherWR;
        private PrivateFlags _flags;

        [SecurityCritical]
        internal ShutDownListener(object target) : this(target, ShutDownEvents.All)
        {
        }

        [SecurityCritical]
        internal ShutDownListener(object target, ShutDownEvents events) : base(target)
        {
            this._flags = (PrivateFlags) ((ushort) (((int) events) | 0x8000));
            if (target == null)
            {
                this._flags = (PrivateFlags) ((ushort) (this._flags | PrivateFlags.Static));
            }
            if (((ushort) (this._flags & PrivateFlags.DomainUnload)) != 0)
            {
                AppDomain.CurrentDomain.DomainUnload += new EventHandler(this.HandleShutDown);
            }
            if (((ushort) (this._flags & PrivateFlags.ProcessExit)) != 0)
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(this.HandleShutDown);
            }
            if (((ushort) (this._flags & PrivateFlags.DispatcherShutdown)) != 0)
            {
                Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
                currentDispatcher.ShutdownFinished += new EventHandler(this.HandleShutDown);
                this._dispatcherWR = new WeakReference(currentDispatcher);
            }
        }

        private void HandleShutDown(object sender, EventArgs e)
        {
            object target = this.Target;
            if ((target == null) && (((ushort) (this._flags & PrivateFlags.Static)) == 0))
            {
                this.StopListening();
            }
            else
            {
                this.OnShutDown(target);
            }
        }

        internal abstract void OnShutDown(object target);
        [SecurityTreatAsSafe, SecurityCritical]
        internal void StopListening()
        {
            if (((ushort) (this._flags & PrivateFlags.Listening)) != 0)
            {
                this._flags = (PrivateFlags) ((ushort) (((int) this._flags) & 0x7fff));
                if (((ushort) (this._flags & PrivateFlags.DomainUnload)) != 0)
                {
                    AppDomain.CurrentDomain.DomainUnload -= new EventHandler(this.HandleShutDown);
                }
                if (((ushort) (this._flags & PrivateFlags.ProcessExit)) != 0)
                {
                    AppDomain.CurrentDomain.ProcessExit -= new EventHandler(this.HandleShutDown);
                }
                if (((ushort) (this._flags & PrivateFlags.DispatcherShutdown)) != 0)
                {
                    Dispatcher target = (Dispatcher) this._dispatcherWR.Target;
                    if (target != null)
                    {
                        target.ShutdownFinished -= new EventHandler(this.HandleShutDown);
                    }
                    this._dispatcherWR = null;
                }
            }
        }

        [Flags]
        private enum PrivateFlags : ushort
        {
            DispatcherShutdown = 4,
            DomainUnload = 1,
            Listening = 0x8000,
            ProcessExit = 2,
            Static = 0x4000
        }
    }
}

