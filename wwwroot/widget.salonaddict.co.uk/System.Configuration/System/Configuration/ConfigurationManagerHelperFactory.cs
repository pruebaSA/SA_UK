namespace System.Configuration
{
    using System;
    using System.Configuration.Internal;
    using System.Security.Permissions;

    internal static class ConfigurationManagerHelperFactory
    {
        private const string ConfigurationManagerHelperTypeString = "System.Configuration.Internal.ConfigurationManagerHelper, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static IConfigurationManagerHelper s_instance;

        [ReflectionPermission(SecurityAction.Assert, Flags=ReflectionPermissionFlag.MemberAccess)]
        private static IConfigurationManagerHelper CreateConfigurationManagerHelper() => 
            System.Configuration.TypeUtil.CreateInstance<IConfigurationManagerHelper>("System.Configuration.Internal.ConfigurationManagerHelper, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

        internal static IConfigurationManagerHelper Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = CreateConfigurationManagerHelper();
                }
                return s_instance;
            }
        }
    }
}

