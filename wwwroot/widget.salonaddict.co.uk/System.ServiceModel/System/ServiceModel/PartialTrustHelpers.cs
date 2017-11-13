namespace System.ServiceModel
{
    using System;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;

    internal static class PartialTrustHelpers
    {
        [SecurityCritical]
        private static Type aptca;
        [SecurityCritical]
        private static SecurityContext aspNetSecurityContext;
        [SecurityCritical]
        private static bool isInitialized;

        [SecurityCritical]
        internal static SecurityContext CaptureSecurityContextNoIdentityFlow()
        {
            if (SecurityContext.IsWindowsIdentityFlowSuppressed())
            {
                return SecurityContext.Capture();
            }
            using (SecurityContext.SuppressFlowWindowsIdentity())
            {
                return SecurityContext.Capture();
            }
        }

        [SecurityCritical]
        private static bool IsAssemblyAptca(Assembly assembly)
        {
            if (aptca == null)
            {
                aptca = typeof(AllowPartiallyTrustedCallersAttribute);
            }
            return (assembly.GetCustomAttributes(aptca, false).Length > 0);
        }

        [SecurityCritical, FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool IsAssemblySigned(Assembly assembly)
        {
            byte[] publicKeyToken = assembly.GetName().GetPublicKeyToken();
            return ((publicKeyToken != null) & (publicKeyToken.Length > 0));
        }

        private static bool IsFullTrust(PermissionSet perms)
        {
            if (perms != null)
            {
                return perms.IsUnrestricted();
            }
            return true;
        }

        [SecurityCritical]
        internal static bool IsTypeAptca(Type type)
        {
            Assembly assembly = type.Assembly;
            if (!IsAssemblyAptca(assembly))
            {
                return !IsAssemblySigned(assembly);
            }
            return true;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void PartialTrustInvoke(ContextCallback callback, object state)
        {
            if (NeedPartialTrustInvoke)
            {
                SecurityContext.Run(aspNetSecurityContext.CreateCopy(), callback, state);
            }
            else
            {
                callback(state);
            }
        }

        internal static bool NeedPartialTrustInvoke
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (!isInitialized)
                {
                    NamedPermissionSet namedPermissionSet = HttpRuntime.GetNamedPermissionSet();
                    if (!IsFullTrust(namedPermissionSet))
                    {
                        try
                        {
                            namedPermissionSet.PermitOnly();
                            aspNetSecurityContext = CaptureSecurityContextNoIdentityFlow();
                        }
                        finally
                        {
                            CodeAccessPermission.RevertPermitOnly();
                        }
                    }
                    isInitialized = true;
                }
                return (aspNetSecurityContext != null);
            }
        }
    }
}

