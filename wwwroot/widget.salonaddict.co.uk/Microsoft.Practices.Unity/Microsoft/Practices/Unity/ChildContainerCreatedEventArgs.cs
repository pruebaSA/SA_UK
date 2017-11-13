namespace Microsoft.Practices.Unity
{
    using System;
    using System.Runtime.CompilerServices;

    public class ChildContainerCreatedEventArgs : EventArgs
    {
        public ChildContainerCreatedEventArgs(ExtensionContext childContext)
        {
            this.ChildContext = childContext;
        }

        public IUnityContainer ChildContainer =>
            this.ChildContext.Container;

        public ExtensionContext ChildContext { get; private set; }
    }
}

