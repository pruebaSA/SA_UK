namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class BuildOperation
    {
        protected BuildOperation(Type typeBeingConstructed)
        {
            this.TypeBeingConstructed = typeBeingConstructed;
        }

        public Type TypeBeingConstructed { get; private set; }
    }
}

