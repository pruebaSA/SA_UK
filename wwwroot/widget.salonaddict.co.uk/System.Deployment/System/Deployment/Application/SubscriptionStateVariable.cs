namespace System.Deployment.Application
{
    using System;

    internal class SubscriptionStateVariable
    {
        public object NewValue;
        public object OldValue;
        public string PropertyName;

        public SubscriptionStateVariable(string name, object newValue, object oldValue)
        {
            this.PropertyName = name;
            this.NewValue = newValue;
            this.OldValue = oldValue;
        }

        public bool IsUnchanged =>
            this.NewValue?.Equals(this.OldValue);
    }
}

