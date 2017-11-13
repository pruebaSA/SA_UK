namespace Microsoft.Practices.Unity
{
    using System;

    public abstract class NamedEventArgs : EventArgs
    {
        private string name;

        protected NamedEventArgs()
        {
        }

        protected NamedEventArgs(string name)
        {
            this.name = name;
        }

        public virtual string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

