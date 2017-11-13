namespace System.ServiceModel.Activation
{
    using System;
    using System.Security;
    using System.ServiceModel;

    internal sealed class HostingMessageProperty
    {
        [SecurityCritical]
        private HostedThreadData currentThreadData;
        [SecurityCritical]
        private HostedImpersonationContext impersonationContext;
        private const string name = "webhost";

        [SecurityCritical]
        internal HostingMessageProperty(HostedHttpRequestAsyncResult result)
        {
            if (!ServiceHostingEnvironment.IsHosted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ProcessNotExecutingUnderHostedContext", new object[] { "HostingMessageProperty" })));
            }
            if (ServiceHostingEnvironment.AspNetCompatibilityEnabled)
            {
                if ((result.ImpersonationContext != null) && result.ImpersonationContext.IsImpersonated)
                {
                    this.impersonationContext = result.ImpersonationContext;
                    this.impersonationContext.AddRef();
                }
                this.currentThreadData = result.HostedThreadData;
            }
        }

        [SecurityCritical]
        internal IDisposable ApplyIntegrationContext()
        {
            if (ServiceHostingEnvironment.AspNetCompatibilityEnabled)
            {
                return this.currentThreadData.CreateContext();
            }
            return null;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        public void Close()
        {
            if (this.impersonationContext != null)
            {
                this.impersonationContext.Release();
                this.impersonationContext = null;
            }
        }

        internal HostedImpersonationContext ImpersonationContext =>
            this.impersonationContext;

        internal static string Name =>
            "webhost";
    }
}

