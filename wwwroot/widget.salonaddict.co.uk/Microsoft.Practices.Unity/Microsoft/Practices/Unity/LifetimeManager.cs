namespace Microsoft.Practices.Unity
{
    using Microsoft.Practices.ObjectBuilder2;
    using System;

    public abstract class LifetimeManager : ILifetimePolicy, IBuilderPolicy
    {
        private bool inUse;

        protected LifetimeManager()
        {
        }

        public abstract object GetValue();
        public abstract void RemoveValue();
        public abstract void SetValue(object newValue);

        internal bool InUse
        {
            get => 
                this.inUse;
            set
            {
                this.inUse = value;
            }
        }
    }
}

