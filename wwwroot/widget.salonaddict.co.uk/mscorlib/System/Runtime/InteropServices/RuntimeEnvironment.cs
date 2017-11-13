namespace System.Runtime.InteropServices
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Text;

    [ComVisible(true)]
    public class RuntimeEnvironment
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern bool FromGlobalAccessCache(Assembly a);
        [DllImport("mscoree.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
        private static extern int GetCORVersion(StringBuilder sb, int BufferLength, ref int retLength);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetDeveloperPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetHostBindingFile();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetModuleFileName();
        public static string GetRuntimeDirectory()
        {
            string runtimeDirectoryImpl = GetRuntimeDirectoryImpl();
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, runtimeDirectoryImpl).Demand();
            return runtimeDirectoryImpl;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string GetRuntimeDirectoryImpl();
        public static string GetSystemVersion()
        {
            StringBuilder sb = new StringBuilder(0x100);
            int retLength = 0;
            if (GetCORVersion(sb, 0x100, ref retLength) == 0)
            {
                return sb.ToString();
            }
            return null;
        }

        public static string SystemConfigurationFile
        {
            get
            {
                StringBuilder builder = new StringBuilder(260);
                builder.Append(GetRuntimeDirectory());
                builder.Append(AppDomainSetup.RuntimeConfigurationFile);
                string path = builder.ToString();
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, path).Demand();
                return path;
            }
        }
    }
}

