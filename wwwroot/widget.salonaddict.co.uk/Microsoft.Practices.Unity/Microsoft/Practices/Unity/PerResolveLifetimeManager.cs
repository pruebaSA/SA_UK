namespace Microsoft.Practices.Unity
{
    using System;

    public class PerResolveLifetimeManager : LifetimeManager
    {
        private readonly object value;

        public PerResolveLifetimeManager()
        {
        }

        internal PerResolveLifetimeManager(object value)
        {
            this.value = value;
        }

        public override object GetValue() => 
            this.value;

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
        }
    }
}

