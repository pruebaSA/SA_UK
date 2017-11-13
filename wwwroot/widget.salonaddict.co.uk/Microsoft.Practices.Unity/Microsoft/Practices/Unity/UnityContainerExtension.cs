namespace Microsoft.Practices.Unity
{
    using System;

    public abstract class UnityContainerExtension : IUnityContainerExtensionConfigurator
    {
        private IUnityContainer container;
        private ExtensionContext context;

        protected UnityContainerExtension()
        {
        }

        protected abstract void Initialize();
        public void InitializeExtension(ExtensionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.container = context.Container;
            this.context = context;
            this.Initialize();
        }

        public virtual void Remove()
        {
        }

        public IUnityContainer Container =>
            this.container;

        protected ExtensionContext Context =>
            this.context;
    }
}

