namespace System.Messaging
{
    using System;
    using System.Messaging.Interop;

    public sealed class SecurityContext : IDisposable
    {
        private bool disposed;
        private SecurityContextHandle handle;

        internal SecurityContext(SecurityContextHandle securityContext)
        {
            this.handle = securityContext;
        }

        public void Dispose()
        {
            this.handle.Close();
            this.disposed = true;
        }

        internal SecurityContextHandle Handle
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(base.GetType().Name);
                }
                return this.handle;
            }
        }
    }
}

