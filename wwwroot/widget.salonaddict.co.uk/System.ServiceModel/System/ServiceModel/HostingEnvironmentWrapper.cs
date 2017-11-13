namespace System.ServiceModel
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Hosting;

    internal static class HostingEnvironmentWrapper
    {
        [MethodImpl(MethodImplOptions.NoInlining), SecurityTreatAsSafe, SecurityCritical]
        public static void DecrementBusyCount()
        {
            HostingEnvironment.DecrementBusyCount();
        }

        public static IDisposable Impersonate() => 
            HostingEnvironment.Impersonate();

        public static IDisposable Impersonate(IntPtr token) => 
            HostingEnvironment.Impersonate(token);

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical, SecurityTreatAsSafe]
        public static void IncrementBusyCount()
        {
            HostingEnvironment.IncrementBusyCount();
        }

        public static string MapPath(string virtualPath) => 
            HostingEnvironment.MapPath(virtualPath);

        [SecurityCritical, SecurityPermission(SecurityAction.Assert, ControlPrincipal=true)]
        public static IDisposable UnsafeImpersonate() => 
            HostingEnvironment.Impersonate();

        [SecurityCritical, SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        public static IDisposable UnsafeImpersonate(IntPtr token) => 
            HostingEnvironment.Impersonate(token);

        [SecurityCritical, SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        public static void UnsafeRegisterObject(IRegisteredObject target)
        {
            HostingEnvironment.RegisterObject(target);
        }

        [SecurityCritical, SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        public static void UnsafeUnregisterObject(IRegisteredObject target)
        {
            HostingEnvironment.UnregisterObject(target);
        }

        public static string ApplicationVirtualPath =>
            HostingEnvironment.ApplicationVirtualPath;

        public static bool IsHosted =>
            HostingEnvironment.IsHosted;

        public static string UnsafeApplicationID =>
            HostingEnvironment.ApplicationID;

        [SecurityCritical, SecurityTreatAsSafe]
        public static System.Web.Hosting.VirtualPathProvider VirtualPathProvider =>
            HostingEnvironment.VirtualPathProvider;
    }
}

