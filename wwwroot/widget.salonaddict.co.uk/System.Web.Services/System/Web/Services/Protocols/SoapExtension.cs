namespace System.Web.Services.Protocols
{
    using System;
    using System.IO;

    public abstract class SoapExtension
    {
        protected SoapExtension()
        {
        }

        public virtual Stream ChainStream(Stream stream) => 
            stream;

        public abstract object GetInitializer(Type serviceType);
        public abstract object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute);
        public abstract void Initialize(object initializer);
        public abstract void ProcessMessage(SoapMessage message);
    }
}

