namespace Microsoft.Practices.Unity
{
    using System;

    public class RegisterEventArgs : NamedEventArgs
    {
        private Microsoft.Practices.Unity.LifetimeManager lifetimeManager;
        private Type typeFrom;
        private Type typeTo;

        public RegisterEventArgs(Type typeFrom, Type typeTo, string name, Microsoft.Practices.Unity.LifetimeManager lifetimeManager) : base(name)
        {
            this.typeFrom = typeFrom;
            this.typeTo = typeTo;
            this.lifetimeManager = lifetimeManager;
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

        public Type TypeFrom
        {
            get => 
                this.typeFrom;
            set
            {
                this.typeFrom = value;
            }
        }

        public Type TypeTo
        {
            get => 
                this.typeTo;
            set
            {
                this.typeTo = value;
            }
        }
    }
}

