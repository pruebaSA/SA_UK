namespace System.Web.Configuration
{
    using Microsoft.Win32;
    using System;
    using System.Threading;
    using System.Web.Compilation;
    using System.Web.Hosting;

    internal static class ServerConfig
    {
        private static int s_iisMajorVersion = 0;
        private static int s_useServerConfig = -1;

        internal static IServerConfig GetInstance()
        {
            if (UseMetabase)
            {
                return MetabaseServerConfig.GetInstance();
            }
            return ProcessHostServerConfig.GetInstance();
        }

        internal static bool UseMetabase
        {
            get
            {
                if (s_iisMajorVersion == 0)
                {
                    int num;
                    try
                    {
                        object obj2 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\InetStp", "MajorVersion", 0);
                        num = (obj2 != null) ? ((int) obj2) : -1;
                    }
                    catch (ArgumentException)
                    {
                        num = -1;
                    }
                    Interlocked.CompareExchange(ref s_iisMajorVersion, num, 0);
                }
                return (s_iisMajorVersion <= 6);
            }
        }

        internal static bool UseServerConfig
        {
            get
            {
                if (s_useServerConfig == -1)
                {
                    int num = 0;
                    if (!HostingEnvironment.IsHosted)
                    {
                        num = 1;
                    }
                    else if (HostingEnvironment.ApplicationHost is ISAPIApplicationHost)
                    {
                        num = 1;
                    }
                    else if (HostingEnvironment.IsUnderIISProcess && !BuildManagerHost.InClientBuildManager)
                    {
                        num = 1;
                    }
                    Interlocked.CompareExchange(ref s_useServerConfig, num, -1);
                }
                return (s_useServerConfig == 1);
            }
        }
    }
}

