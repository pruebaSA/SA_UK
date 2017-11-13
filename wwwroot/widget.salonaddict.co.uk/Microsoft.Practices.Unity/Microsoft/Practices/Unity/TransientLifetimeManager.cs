namespace Microsoft.Practices.Unity
{
    using System;

    public class TransientLifetimeManager : LifetimeManager
    {
        public override object GetValue() => 
            null;

        public override void RemoveValue()
        {
        }

        public override void SetValue(object newValue)
        {
        }
    }
}

