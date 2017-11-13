namespace Microsoft.Practices.Unity
{
    using System;

    public class ContainerControlledLifetimeManager : SynchronizedLifetimeManager, IDisposable
    {
        private object value;

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (this.value != null)
            {
                if (this.value is IDisposable)
                {
                    ((IDisposable) this.value).Dispose();
                }
                this.value = null;
            }
        }

        public override void RemoveValue()
        {
            this.Dispose();
        }

        protected override object SynchronizedGetValue() => 
            this.value;

        protected override void SynchronizedSetValue(object newValue)
        {
            this.value = newValue;
        }
    }
}

