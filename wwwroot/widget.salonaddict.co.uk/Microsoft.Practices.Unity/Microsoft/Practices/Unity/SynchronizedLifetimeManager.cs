namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;
    using System.Threading;

    public abstract class SynchronizedLifetimeManager : LifetimeManager, IRequiresRecovery
    {
        private object lockObj = new object();

        protected SynchronizedLifetimeManager()
        {
        }

        public override object GetValue()
        {
            Monitor.Enter(this.lockObj);
            object obj2 = this.SynchronizedGetValue();
            if (obj2 != null)
            {
                Monitor.Exit(this.lockObj);
            }
            return obj2;
        }

        public void Recover()
        {
            this.TryExit();
        }

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
            this.SynchronizedSetValue(newValue);
            this.TryExit();
        }

        protected abstract object SynchronizedGetValue();
        protected abstract void SynchronizedSetValue(object newValue);
        private void TryExit()
        {
            try
            {
                Monitor.Exit(this.lockObj);
            }
            catch (SynchronizationLockException)
            {
            }
        }
    }
}

