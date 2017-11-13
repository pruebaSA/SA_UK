namespace Microsoft.Practices.Unity
{
    using System;

    public class ExternallyControlledLifetimeManager : LifetimeManager
    {
        private WeakReference value = new WeakReference(null);

        public override object GetValue() => 
            this.value.Target;

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
            this.value = new WeakReference(newValue);
        }
    }
}

