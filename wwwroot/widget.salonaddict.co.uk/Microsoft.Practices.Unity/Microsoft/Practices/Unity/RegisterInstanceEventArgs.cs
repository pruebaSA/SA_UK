namespace Microsoft.Practices.Unity
{
    using System;

    public class RegisterInstanceEventArgs : NamedEventArgs
    {
        private object instance;
        private Microsoft.Practices.Unity.LifetimeManager lifetimeManager;
        private Type registeredType;

        public RegisterInstanceEventArgs()
        {
        }

        public RegisterInstanceEventArgs(Type registeredType, object instance, string name, Microsoft.Practices.Unity.LifetimeManager lifetimeManager) : base(name)
        {
            this.registeredType = registeredType;
            this.instance = instance;
            this.lifetimeManager = lifetimeManager;
        }

        public object Instance
        {
            get => 
                this.instance;
            set
            {
                this.instance = value;
            }
        }

        public Microsoft.Practices.Unity.LifetimeManager LifetimeManager
        {
            get => 
                this.lifetimeManager;
            set
            {
                this.lifetimeManager = value;
            }
        }

        public Type RegisteredType
        {
            get => 
                this.registeredType;
            set
            {
                this.registeredType = value;
            }
        }
    }
}

