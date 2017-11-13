namespace System.Threading
{
    using System;

    public class HostExecutionContext
    {
        private object state;

        public HostExecutionContext()
        {
        }

        public HostExecutionContext(object state)
        {
            this.state = state;
        }

        public virtual HostExecutionContext CreateCopy()
        {
            if (this.state is IUnknownSafeHandle)
            {
                ((IUnknownSafeHandle) this.state).Clone();
            }
            return new HostExecutionContext(this.state);
        }

        protected internal object State
        {
            get => 
                this.state;
            set
            {
                this.state = value;
            }
        }
    }
}

