﻿namespace System.Web.Configuration
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Hosting;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public static class WebConfigurationManager
    {
        public static object GetSection(string sectionName)
        {
            if (HttpConfigurationSystem.UseHttpConfigurationSystem)
            {
                return HttpConfigurationSystem.GetSection(sectionName);
            }
            return ConfigurationManager.GetSection(sectionName);
        }

        public static object GetSection(string sectionName, string path)
        {
            if (!HttpConfigurationSystem.UseHttpConfigurationSystem)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("Config_GetSectionWithPathArgInvalid"));
            }
            return HttpConfigurationSystem.GetSection(sectionName, path);
        }

        public static object GetWebApplicationSection(string sectionName)
        {
            if (HttpConfigurationSystem.UseHttpConfigurationSystem)
            {
                return HttpConfigurationSystem.GetApplicationSection(sectionName);
            }
            return ConfigurationManager.GetSection(sectionName);
        }

        public static System.Configuration.Configuration OpenMachineConfiguration() => 
            OpenWebConfigurationImpl(WebLevel.Machine, null, null, null, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMachineConfiguration(string locationSubPath) => 
            OpenWebConfigurationImpl(WebLevel.Machine, null, null, null, locationSubPath, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMachineConfiguration(string locationSubPath, string server) => 
            OpenWebConfigurationImpl(WebLevel.Machine, null, null, null, locationSubPath, server, null, null, IntPtr.Zero);

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static System.Configuration.Configuration OpenMachineConfiguration(string locationSubPath, string server, IntPtr userToken) => 
            OpenWebConfigurationImpl(WebLevel.Machine, null, null, null, locationSubPath, server, null, null, userToken);

        public static System.Configuration.Configuration OpenMachineConfiguration(string locationSubPath, string server, string userName, string password) => 
            OpenWebConfigurationImpl(WebLevel.Machine, null, null, null, locationSubPath, server, userName, password, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap) => 
            OpenWebConfigurationImpl(WebLevel.Machine, fileMap, null, null, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMappedMachineConfiguration(ConfigurationFileMap fileMap, string locationSubPath) => 
            OpenWebConfigurationImpl(WebLevel.Machine, fileMap, null, null, locationSubPath, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMappedWebConfiguration(WebConfigurationFileMap fileMap, string path) => 
            OpenWebConfigurationImpl(WebLevel.Path, fileMap, path, null, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMappedWebConfiguration(WebConfigurationFileMap fileMap, string path, string site) => 
            OpenWebConfigurationImpl(WebLevel.Path, fileMap, path, site, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenMappedWebConfiguration(WebConfigurationFileMap fileMap, string path, string site, string locationSubPath) => 
            OpenWebConfigurationImpl(WebLevel.Path, fileMap, path, site, locationSubPath, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenWebConfiguration(string path) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, null, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenWebConfiguration(string path, string site) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, site, null, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenWebConfiguration(string path, string site, string locationSubPath) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, site, locationSubPath, null, null, null, IntPtr.Zero);

        public static System.Configuration.Configuration OpenWebConfiguration(string path, string site, string locationSubPath, string server) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, site, locationSubPath, server, null, null, IntPtr.Zero);

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static System.Configuration.Configuration OpenWebConfiguration(string path, string site, string locationSubPath, string server, IntPtr userToken) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, site, locationSubPath, server, null, null, userToken);

        public static System.Configuration.Configuration OpenWebConfiguration(string path, string site, string locationSubPath, string server, string userName, string password) => 
            OpenWebConfigurationImpl(WebLevel.Path, null, path, site, locationSubPath, server, userName, password, IntPtr.Zero);

        private static System.Configuration.Configuration OpenWebConfigurationImpl(WebLevel webLevel, ConfigurationFileMap fileMap, string path, string site, string locationSubPath, string server, string userName, string password, IntPtr userToken)
        {
            VirtualPath path2;
            if (HostingEnvironment.IsHosted)
            {
                path2 = VirtualPath.CreateNonRelativeAllowNull(path);
            }
            else
            {
                path2 = VirtualPath.CreateAbsoluteAllowNull(path);
            }
            return WebConfigurationHost.OpenConfiguration(webLevel, fileMap, path2, site, locationSubPath, server, userName, password, userToken);
        }

        public static NameValueCollection AppSettings =>
            ConfigurationManager.AppSettings;

        public static ConnectionStringSettingsCollection ConnectionStrings =>
            ConfigurationManager.ConnectionStrings;
    }
}

