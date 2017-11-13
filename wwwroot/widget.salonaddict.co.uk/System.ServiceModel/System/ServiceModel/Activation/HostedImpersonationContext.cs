namespace System.ServiceModel.Activation
{
    using System;
    using System.ComponentModel;
    using System.IdentityModel;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.ComIntegration;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class HostedImpersonationContext
    {
        [SecurityCritical]
        private bool isImpersonated;
        [SecurityCritical]
        private int refCount;
        [SecurityCritical]
        private SafeCloseHandleCritical tokenHandle;

        [SecurityCritical]
        public HostedImpersonationContext()
        {
            if (ServiceHostingEnvironment.AspNetCompatibilityEnabled)
            {
                bool flag = SafeNativeMethods.OpenCurrentThreadTokenCritical(SafeNativeMethods.GetCurrentThread(), TokenAccessLevels.Query | TokenAccessLevels.Impersonate, true, out this.tokenHandle);
                int error = Marshal.GetLastWin32Error();
                if (flag)
                {
                    this.isImpersonated = true;
                    Interlocked.Increment(ref this.refCount);
                }
                else
                {
                    System.ServiceModel.Diagnostics.Utility.CloseInvalidOutSafeHandleCritical(this.tokenHandle);
                    this.tokenHandle = null;
                    if (error != 0x3f0)
                    {
                        throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new Win32Exception(error, System.ServiceModel.SR.GetString("Hosting_ImpersonationFailed")));
                    }
                }
            }
        }

        [SecurityCritical]
        public void AddRef()
        {
            if (this.IsImpersonated)
            {
                Interlocked.Increment(ref this.refCount);
            }
        }

        [SecurityCritical]
        public IDisposable Impersonate()
        {
            if (!this.isImpersonated)
            {
                return null;
            }
            HostedInnerImpersonationContext context = null;
            lock (this.tokenHandle)
            {
                context = HostedInnerImpersonationContext.UnsafeCreate(this.tokenHandle.DangerousGetHandle());
                GC.KeepAlive(this.tokenHandle);
            }
            return context;
        }

        [SecurityCritical]
        public void Release()
        {
            if (this.IsImpersonated && (Interlocked.Decrement(ref this.refCount) == 0))
            {
                lock (this.tokenHandle)
                {
                    this.tokenHandle.Close();
                    this.tokenHandle = null;
                }
            }
        }

        public bool IsImpersonated =>
            this.isImpersonated;

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class HostedInnerImpersonationContext : IDisposable
        {
            private IDisposable impersonatedContext;

            private HostedInnerImpersonationContext(IDisposable impersonatedContext)
            {
                if (impersonatedContext == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ImpersonationFailed")));
                }
                this.impersonatedContext = impersonatedContext;
            }

            public void Dispose()
            {
                if (this.impersonatedContext != null)
                {
                    this.impersonatedContext.Dispose();
                    this.impersonatedContext = null;
                }
            }

            public static HostedImpersonationContext.HostedInnerImpersonationContext UnsafeCreate(IntPtr token) => 
                new HostedImpersonationContext.HostedInnerImpersonationContext(HostingEnvironmentWrapper.UnsafeImpersonate(token));
        }
    }
}

