namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Proxies;
    using System.ServiceModel;

    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class MonikerProxyAttribute : ProxyAttribute, ICustomFactory
    {
        public override MarshalByRefObject CreateInstance(Type serverType)
        {
            if (serverType != typeof(ServiceMoniker))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return MonikerBuilder.CreateMonikerInstance();
        }

        MarshalByRefObject ICustomFactory.CreateInstance(Type serverType)
        {
            if (serverType != typeof(ServiceMoniker))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return MonikerBuilder.CreateMonikerInstance();
        }
    }
}

